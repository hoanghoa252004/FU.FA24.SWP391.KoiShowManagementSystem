using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories.Helper;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private KoiShowManagementSystemContext _context;
        private readonly S3UploadService _s3Service;
        public ShowRepository(KoiShowManagementSystemContext context, S3UploadService s3Service)
        {
            this._context = context;
            this._s3Service = s3Service;
        }



        public Task<(int TotalItems, List<KoiModel>)> GetKoiByShowIdAsync(int pageIndex, int pageSize, int showId)
        {
            throw new NotImplementedException();
        }

        public Task<KoiModel?> GetKoiDetailAsync(int koiId)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationFormModel?> GetRegistrationFormAsync(int showId)
        {
            throw new NotImplementedException();
        }

        public Task<ShowModel?> GetShowDetailsAsync(int showId)
        {
            throw new NotImplementedException();
        }

        public Task<(int TotalItems, List<ShowModel>)> SearchShowAsync(int pageIndex, int pageSize, string keyword)
        {
            throw new NotImplementedException();
        }





        /*
public async Task<ShowModel?> GetShowDetailsAsync(int showId)
{
   var result = await (from sho in _context.Shows
                       where sho.Id == showId
                       select new ShowModel
                       {
                           ShowId = sho.Id,
                           ShowTitle = sho.Title,
                           ShowBanner = sho.Banner!,
                           ShowDesc = sho.Description!,
                           StartDate = sho.ScoreStartDate,
                           RegistrationStartDate = sho.RegisterStartDate,
                           RegistrationCloseDate = sho.RegisterEndDate,
                           ShowStatus = sho.Status!,
                           EndDate = sho.ScoreEndDate,

                           ShowGroups = (from gro in _context.Groups
                                         where gro.ShowId == sho.Id
                                         select new GroupModel
                                         {
                                             GroupId = gro.Id,
                                             GroupName = gro.Name,
                                             KoiDetails = (from koi in _context.KoiRegistrations
                                                           where koi.GroupId == gro.Id
                                                           select new KoiModel
                                                           {
                                                               KoiID = koi.Id,
                                                               KoiName = koi.Name,
                                                               Rank = koi.Rank,
                                                               IsBestVote = koi.IsBestVote
                                                           }).ToList()
                                         }).ToList(),

                           ShowReferee = (from refDetail in _context.RefereeDetails
                                          join usr in _context.Users on refDetail.UserId equals usr.Id into referees
                                          from usr in referees.DefaultIfEmpty()
                                          where refDetail.ShowId == sho.Id
                                          select new RefereeModel
                                          {
                                              RefereeId = refDetail.Id,
                                              RefereeName = usr != null ? usr.Name : "No Referee"
                                          }).ToList()
                       }).FirstOrDefaultAsync();

   return result;
}


public async Task<(int TotalItems, List<ShowModel>)> SearchShowAsync(int pageIndex, int pageSize, string keyword)
{
   var query = _context.Shows.Where(s => s.Title.Contains(keyword));

   var totalItems = await query.CountAsync();

   var shows = await query
       .Skip((pageIndex - 1) * pageSize)
       .Take(pageSize)
       .Select(s => new ShowModel
       {
           ShowId = s.Id,
           ShowTitle = s.Title,
           ShowBanner = s.Banner,
           ShowStatus = s.Status

       }).ToListAsync();

   return (totalItems, shows);
}

public async Task<KoiModel?> GetKoiDetailAsync(int koiId)
{
   var result = await (from kr in _context.KoiRegistrations
                       join u in _context.Users on kr.UserId equals u.Id into users
                       from u in users.DefaultIfEmpty()
                       join g in _context.Groups on kr.GroupId equals g.Id into groups
                       from g in groups.DefaultIfEmpty()
                       join s in _context.Shows on g.ShowId equals s.Id into shows
                       from s in shows.DefaultIfEmpty()
                       join v in _context.Varieties on kr.VarietyId equals v.Id into varieties
                       from v in varieties.DefaultIfEmpty()
                       where kr.Id == koiId && (kr.Size >= g.SizeMin && kr.Size <= g.SizeMax) || g == null
                       select new KoiModel
                       {
                           KoiID = kr.Id,
                           KoiName = kr.Name,
                           KoiImg = _context.Illustrations.Where(i => i.KoiId == kr.Id).Select(i => i.ImageUrl).FirstOrDefault(),
                           KoiVideo = _context.Illustrations.Where(i => i.KoiId == kr.Id).Select(i => i.VideoUrl).FirstOrDefault(),
                           KoiVariety = v != null ? v.Name : "Unknown Variety", // Handle null variety
                           KoiDesc = kr.Description,
                           KoiSize = kr.Size,
                           TotalScore = kr.TotalScore,
                           IsBestVote = kr.IsBestVote,
                           RegistrationStatus = kr.Status,
                           Rank = kr.Rank
                       }).FirstOrDefaultAsync();

   return result;
}


public async Task<(int TotalItems, List<KoiModel>)> GetKoiByShowIdAsync(int pageIndex, int pageSize, int showId)
{
   var query = _context.KoiRegistrations
       .Include(k => k.Group)
       .Include(k => k.Variety)
       .Include(k => k.Illustration)
       .Where(k => k.Group.ShowId == showId && k.IsPaid == true); // Filter for IsPaid = true

   var totalItems = await query.CountAsync();

   var koiList = await query
       .Select(k => new KoiModel
       {
           KoiID = k.Id,
           KoiName = k.Name,
           KoiImg = k.Illustration != null ? k.Illustration.ImageUrl : null,
           KoiVariety = k.Variety != null ? k.Variety.Name : "Unknown",
           KoiSize = k.Size,
           TotalScore = k.TotalScore,
           IsBestVote = k.IsBestVote,
           KoiStatus = k.Status,
           Rank = k.Rank,
           GroupName = k.Group.Name

       })
       .Skip((pageIndex - 1) * pageSize)
       .Take(pageSize)
       .ToListAsync();

   return (totalItems, koiList);
}
*/



        public async Task<List<ShowModel>> GetClosestShowAsync()
        {
            var shows = await _context.Shows
                .Where(s => s.Status != "Draft")
                .Include(s => s.Groups)
                .OrderByDescending(s => s.RegisterStartDate).Take(5)
                .Select(s => new ShowModel
                {
                    ShowId = s.Id,
                    ShowTitle = s.Title,
                    ShowBanner = s.Banner,
                    ShowDesc = s.Description,
                    ShowStatus = s.Status
                }).ToListAsync();

            var firstShow = shows.FirstOrDefault();

            if (firstShow == null)
            {
                throw new Exception("No show found");
            }

            if (firstShow.ShowStatus == "Finished")
            {
                var groups  = _context.Groups
                   .Where(g => g.ShowId == firstShow.ShowId)
                   .Select(g => new GroupModel
                   {
                       GroupId = g.Id,
                       GroupName = g.Name,
                       KoiDetails = g.Registrations
                           .Where(r => r.Rank == 1 || r.Rank == 2 || r.Rank == 3 || r.IsBestVote == true)
                           .OrderBy(r=> r.Rank)
                           .Select(r => new RegistrationModel
                           {
                               Id = r.Id,
                               Name = r.Koi.Name,
                               Rank = r.Rank,
                               IsBestVote = r.IsBestVote
                           }).ToList()
                   }).ToList();
                if (!groups.IsNullOrEmpty())
                {
                    firstShow.ShowGroups = groups;
                }
            }
            else
            {
                firstShow.ShowReferee = _context.RefereeDetails
                    .Where(r => r.ShowId == firstShow.ShowId)
                    .Include(r => r.User)
                    .Select(r => new RefereeModel
                    {
                        RefereeId = r.Id,
                        RefereeName = r.User.Name
                    }).ToList();
            }
            return shows;
        }

        public async Task<int> AddNewShow(ShowDTO dto)
        {
            int result = 0;
            Show show = new()
            {
                Title = dto.Title,
                Description = dto.Description,
                ScoreStartDate = dto.ScoreStartDate,
                RegisterStartDate = dto.RegisterStartDate,
                RegisterEndDate = dto.RegisterEndDate,
                ScoreEndDate = dto.ScoreEndDate,
                Banner = await _s3Service.UploadShowBannerImage(dto.Banner),
                Status = "up comming",
                Groups = dto.Groups.Select(g => new Group
                {
                    Name = g.Name,
                    SizeMin = g.MinSize,
                    SizeMax = g.MaxSize,
                    Varieties = _context.Varieties.Where(v => g.Varieties.Contains(v.Id)).ToList(),
                    Criteria = g.Criterias.Select(c => new Criterion
                    {
                        Name = c.Name,
                        Percentage = c.Percentage,
                        Description = c.Description,
                        Status = true,
                    }).ToList()
                }).ToList()
            };

            _context.Shows.Add(show);         
            _context.Groups.AddRange(show.Groups);            
            _context.Criteria.AddRange(show.Groups.SelectMany(g => g.Criteria));
            result = await _context.SaveChangesAsync();
            return result;
        }

        public async Task<List<VarietyModel>> GetAllVarietiesAsync()
        {
            var reuslt =await  _context.Varieties.Select(v => new VarietyModel
            {
                VarietyId = v.Id,
                VarietyName = v.Name
            }).ToListAsync();
            return reuslt;
        }




        /*
        public async Task<List<ShowModel>> GetClosestShowAsync()
        {
           var shows = await _context.Shows
               .Where(s => s.Status != "Draft")
               .Include(s => s.Groups)
               .OrderByDescending(s => s.RegisterStartDate).Take(5)
               .Select(s => new ShowModel
               {
                   ShowId = s.Id,
                   ShowTitle = s.Title,
                   ShowBanner = s.Banner,
                   ShowDesc = s.Description,
                   ShowStatus = s.Status
               }).ToListAsync();
           // get the first show
           var firstShow = shows.FirstOrDefault();
           if (firstShow == null)
           {
               throw new Exception("No show found");
           }
           if (firstShow.ShowStatus == "Finished")
           {
               //get group of firstShow  and top rank 1,2,3, best vote of each groupModel
               firstShow.ShowGroups = _context.Groups
                   .Where(g => g.ShowId == firstShow.ShowId)
                   .Include(g => g.KoiRegistrations)
                   .Select(g => new GroupModel
                   {
                       GroupId = g.Id,
                       GroupName = g.Name,
                       KoiDetails = g.KoiRegistrations
                           .Where(k => k.Rank == 1 || k.Rank == 2 || k.Rank == 3 || k.IsBestVote == true)
                           .OrderBy(k => k.Rank)
                           .Select(k => new KoiModel
                           {
                               KoiID = k.Id,
                               KoiName = k.Name,
                               Rank = k.Rank,
                               IsBestVote = k.IsBestVote
                           }).ToList()
                   }).ToList();
           }
           else
           {
               firstShow.ShowReferee = _context.RefereeDetails
                   .Where(r => r.ShowId == firstShow.ShowId)
                   .Include(r => r.User)
                   .Select(r => new RefereeModel
                   {
                       RefereeId = r.Id,
                       RefereeName = r.User.Name
                   }).ToList();
           }


           return shows;
        }

        public async Task<RegistrationFormModel?> GetRegistrationFormAsync(int showId)
        {
           // Lấy show:
           var show = await _context.Shows.SingleOrDefaultAsync(show => show.Id == showId);
           // Tìm những groups của show để lấy bảng size:
           var groupList = _context.Groups.Where(group => group.ShowId == showId);
           List<GroupModel> sizeList = new List<GroupModel>();
           List<VarietyModel> varietyModels = new List<VarietyModel>();
           var varietiesList = _context.Varieties.Include(var => var.Groups).ToList();

           foreach (var grp in groupList)
           {
               foreach (var var in varietiesList)
               {
                   if (grp.Varieties.Contains(var))
                   {
                       varietyModels.Add(new VarietyModel()
                       {
                           VarietyId = var.Id,
                           VarietyName = var.Name,
                       });
                       sizeList.Add(new GroupModel()
                       {
                           GroupId = grp.Id,
                           GroupName = grp.Name,
                           SizeMax = grp.SizeMax,
                           SizeMin = grp.SizeMin,
                           Unit = "cm"
                       });
                   }
               }
           }

           return new RegistrationFormModel()
           {
               ShowId = showId,
               ShowName = show!.Title,
               SizeList = sizeList,
               VarietyList = varietyModels,
           };
        }
        */
    }
}