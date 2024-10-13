﻿using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories.Helper;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly S3UploadService _s3UploadService;
        private KoiShowManagementSystemContext _context;
        public RegistrationRepository(KoiShowManagementSystemContext context, S3UploadService _s3UploadService)
        {
            this._context = context;
            this._s3UploadService = _s3UploadService;
        }

        public async Task CreateRegistrationAsync(RegistrationFormModel dto)
        {
            string image1 = await _s3UploadService.UploadRegistrationImage(dto.Image1!);
            string image2 = await _s3UploadService.UploadRegistrationImage(dto.Image2!);
            string image3 = await _s3UploadService.UploadRegistrationImage(dto.Image3!);
            Registration newRegistration = new Registration()
            {
                CreateDate = DateOnly.FromDateTime(DateTime.Now),
                Size = (int)dto.Size!,
                KoiId = (int)dto.KoiId!,
                GroupId = dto.GroupId,
            };
            await _context.Set<Registration>().AddAsync(newRegistration);
            await _context.SaveChangesAsync();
            // Add Media:
            await _context.Set<Media>().AddAsync(new Media()
            {
                RegistrationId = newRegistration.Id,
                Image1 = image1,
                Image2 = image2,
                Image3 = image3,
                Video = dto.Video!,
            });
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RegistrationModel>> GetRegistrationByUserIdAsync(int id)
        {
            // Lấy Koi của User:
            IEnumerable<Koi> kois = (await _context.Set<Koi>().ToListAsync())
                        .Where(koiRegist => koiRegist.UserId == id);
            // Join lấy thông tin:
            IEnumerable<Registration> registrations = await _context.Set<Registration>().ToListAsync();
            IEnumerable<Show> shows = await _context.Set<Show>().ToListAsync();
            IEnumerable<Group> groups = await _context.Set<Group>().ToListAsync();
            IEnumerable<Media> media = await _context.Set<Media>().ToListAsync();
            IEnumerable<Variety> varieties = await _context.Set<Variety>().ToListAsync();
            IEnumerable<RegistrationModel> result = from koi in kois
                         join var in varieties on koi.VarietyId equals var.Id
                         join regist in registrations on  koi.Id equals regist.KoiId
                         join med in media on regist.Id equals med.RegistrationId
                         join grp in groups on regist.GroupId equals grp.Id
                         join show in shows on grp.ShowId equals show.Id
                         select new RegistrationModel()
                         {
                            Id = koi.Id,
                            Name = koi.Name,
                            Description = koi.Description,
                            Size = koi.Size,
                            Variety = var.Name,
                            ShowId = show.Id,
                            Show = show.Title,
                            Group = grp.Name,
                            CreateDate = regist.CreateDate,
                            Rank = regist.Rank,
                            TotalScore = regist.TotalScore,
                            Status = regist.Status,
                            IsBestVote = regist.IsBestVote,
                            Image1 = med.Image1,
                            Image2 = med.Image1,
                            Image3 = med.Image1,
                            Video = med.Video,
                         };
            return result;
        }

        public async Task<List<RegistrationModel>> GetRegistrationByShowAsync(int showId)
        {
            var query = _context.Registrations
                .Include(r => r.Koi) // Include Koi
                .Include(r => r.Group)
                .Include(r => r.Group!.Varieties) // Ensure Varieties can be included if needed
                .Include(r => r.Media) // Include Media for images/videos
                .Where(r => r.Group!.ShowId == showId); // Filter for IsPaid = true


            var koiList = await query
                .Select(r => new RegistrationModel
                {
                    KoiID = r.Koi!.Id,
                    Name = r.Koi.Name,
                    Image1 = r.Media!.Image1,
                    Variety = r.Koi.Variety.Name,
                    Size = r.Koi.Size,
                    TotalScore = r.TotalScore,
                    IsBestVote = r.IsBestVote,
                    Status = r.Status,
                    Rank = r.Rank,
                    Group = r.Group!.Name,
                    Id = r.Id,
                    IsPaid = r.IsPaid,
                }).ToListAsync();

            return koiList;
        }

        public async Task<RegistrationModel?> GetRegistrationAsync(int registrationId)
        {
            var result = await (from reg in _context.Registrations
                                join koi in _context.Kois on reg.KoiId equals koi.Id into koiGroup
                                from koi in koiGroup.DefaultIfEmpty()
                                join user in _context.Users on koi.UserId equals user.Id into users
                                from user in users.DefaultIfEmpty()
                                join g in _context.Groups on reg.GroupId equals g.Id into groups
                                from g in groups.DefaultIfEmpty()
                                join s in _context.Shows on g.ShowId equals s.Id into shows
                                from s in shows.DefaultIfEmpty()
                                join v in _context.Varieties on koi.VarietyId equals v.Id into varieties
                                from v in varieties.DefaultIfEmpty()
                                where reg.Id == registrationId
                                select new RegistrationModel
                                {
                                    KoiID = koi.Id,
                                    Name = koi.Name,
                                    Image1 = _context.Media.Where(m => m.RegistrationId == reg.Id).Select(m => m.Image1).FirstOrDefault(),
                                    Image2 = _context.Media.Where(m => m.RegistrationId == reg.Id).Select(m => m.Image2).FirstOrDefault(),
                                    Image3 = _context.Media.Where(m => m.RegistrationId == reg.Id).Select(m => m.Image3).FirstOrDefault(),
                                    Video = _context.Media.Where(m => m.RegistrationId == reg.Id).Select(m => m.Video).FirstOrDefault(),
                                    Variety = v != null ? v.Name : "Unknown Variety",
                                    Description = koi.Description,
                                    Size = koi.Size,
                                    TotalScore = reg.TotalScore,
                                    IsBestVote = reg.IsBestVote,
                                    Status = reg.Status,
                                    Rank = reg.Rank,
                                    Id = reg.Id,
                                }).FirstOrDefaultAsync();

            return result!;
        }

        public async Task UpdateRegistrationAsync(RegistrationFormModel dto)
        {
            var updateRegistration = await _context.Registrations.SingleOrDefaultAsync(r => r.Id == dto.Id);
            if(updateRegistration != null)
            {
                if(dto.Size != null)
                {
                    updateRegistration.Size = (int)dto.Size;
                }
                if(dto.KoiId != null)
                {
                    updateRegistration.KoiId = (int)dto.KoiId;
                }
                if(dto.GroupId != null)
                {
                    updateRegistration.GroupId = dto.GroupId;
                }
            }
            var updateMedia = await _context.Media.SingleOrDefaultAsync(m => m.RegistrationId == dto.Id);
            if(updateMedia != null)
            {
                if(dto.Video != null)
                {
                    updateMedia.Video = dto.Video;
                }
                if(dto.Image1 != null)
                {
                    updateMedia.Image1 = await _s3UploadService.UpdateImageAsync(updateMedia.Image1, dto.Image1);
                }
                if (dto.Image2 != null)
                {
                    updateMedia.Image1 = await _s3UploadService.UpdateImageAsync(updateMedia.Image2, dto.Image2);
                }
                if (dto.Image3 != null)
                {
                    updateMedia.Image1 = await _s3UploadService.UpdateImageAsync(updateMedia.Image3, dto.Image3);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RegistrationModel>> GetAllRegistrationAsync()
        {
            var query = _context.Registrations
                .Include(r => r.Koi) // Include Koi
                .Include(r => r.Group)
                .Include(r => r.Group!.Varieties) // Ensure Varieties can be included if needed
                .Include(r => r.Media); // Include Media for images/videos

            // r.Media.FirstOrDefault() != null ? r.Media.FirstOrDefault()!.Image1 : null,
            var koiList = await query
                .Select(r => new RegistrationModel
                {
                    KoiID = r.Koi!.Id,
                    Name = r.Koi.Name,
                    Image1 = (from med in _context.Media
                             where med.RegistrationId ==r.Id
                             select med.Image1).First(),
                    Image2 = (from med in _context.Media
                              where med.RegistrationId == r.Id
                              select med.Image2).First(),
                    Image3 = (from med in _context.Media
                              where med.RegistrationId == r.Id
                              select med.Image3).First(),
                    Variety = r.Koi.Variety != null ? r.Koi.Variety.Name : "Unknown",
                    Size = r.Koi.Size,
                    TotalScore = r.TotalScore,
                    IsBestVote = r.IsBestVote,
                    Status = r.Status,
                    Rank = r.Rank,
                    Group = r.Group!.Name ?? "Unknown Group",
                    Id = r.Id,
                    IsPaid = r.IsPaid,
                    Video = (from med in _context.Media
                             where med.RegistrationId == r.Id
                             select med.Video).First(),
                    GroupId = r.Group!.Id,
                }).ToListAsync();
            return koiList;
        }
    }
}
