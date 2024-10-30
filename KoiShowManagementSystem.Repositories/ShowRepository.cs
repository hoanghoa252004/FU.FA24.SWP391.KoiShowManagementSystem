using Amazon.S3.Model.Internal.MarshallTransformations;
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
using System.Reflection.Metadata.Ecma335;
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

        public async Task<ShowModel?> GetShowDetailsAsync(int showId)
        {
            var result = await (from sho in _context.Shows
                                where sho.Id == showId
                                select new ShowModel
                                {
                                    ShowId = sho.Id,
                                    ShowTitle = sho.Title,
                                    ShowBanner = sho.Banner ?? string.Empty,
                                    ShowDesc = sho.Description ?? string.Empty,
                                    StartDate = sho.ScoreStartDate,
                                    RegistrationStartDate = sho.RegisterStartDate,
                                    RegistrationCloseDate = sho.RegisterEndDate,
                                    ShowStatus = sho.Status ?? string.Empty,
                                    EndDate = sho.ScoreEndDate,
                                    EntranceFee = sho.EntranceFee,
                                    ShowGroups = (from gro in _context.Groups
                                                  where gro.ShowId == sho.Id && gro.Status == true
                                                  select new GroupModel
                                                  {
                                                      GroupId = gro.Id,
                                                      GroupName = gro.Name ?? string.Empty,
                                                      Registrations = (from reg in _context.Registrations
                                                                    where reg.GroupId == gro.Id
                                                                    select new RegistrationModel
                                                                    {
                                                                        KoiID = reg.KoiId ?? 0,
                                                                        Name = reg.Koi != null ? reg.Koi.Name : "Unknown",
                                                                        Rank = reg.Rank,
                                                                        TotalScore = reg.TotalScore,
                                                                        IsBestVote = reg.IsBestVote
                                                                    }).OrderBy(r => r.Rank).ToList(),
                                                      Criterion = (from cri in _context.Criteria
                                                                   where cri.GroupId == gro.Id
                                                                   select new CriterionModel()
                                                                   {
                                                                       Id = cri.Id,
                                                                       Name = cri.Name,    
                                                                       Description = cri.Description,
                                                                       Percentage = cri.Percentage,
                                                                   }).ToList(),
                                                  }).ToList(),
                                    ShowReferee = (from refDetail in _context.RefereeDetails
                                                   join usr in _context.Users on refDetail.UserId equals usr.Id into referees
                                                   from usr in referees.DefaultIfEmpty()
                                                   where refDetail.ShowId == sho.Id
                                                   select new RefereeModel
                                                   {
                                                       RefereeId = refDetail.Id,
                                                       RefereeName = usr != null ? usr.Name : "No Referee"
                                                   }).ToList(),
                                }).FirstOrDefaultAsync();

            return result;
        }


        public async Task<(int TotalItems, List<ShowModel>)> SearchShowAsync(int pageIndex, int pageSize, string keyword)
        {
            var query = _context.Shows.Where(s => s.Title.Contains(keyword)&&(s.Status.ToLower() != "up comming"));

            var totalItems = await query.CountAsync();

            var shows = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new ShowModel
                {
                    ShowId = s.Id,
                    ShowTitle = s.Title,
                    ShowBanner = s.Banner,
                    ShowStatus = s.Status, 
                    EntranceFee = s.EntranceFee,
                }).ToListAsync();

            return (totalItems, shows);
        }

        public async Task<List<ShowModel>> GetClosestShowAsync()
        {
            var shows = await _context.Shows
                .Where(s => s.Status.ToLower() != "up comming")
                .Include(s => s.Groups)
                .OrderByDescending(s => s.RegisterStartDate).Take(5)
                .Select(s => new ShowModel
                {
                    ShowId = s.Id,
                    ShowTitle = s.Title,
                    RegistrationStartDate = s.RegisterStartDate,
                    RegistrationCloseDate = s.RegisterEndDate,
                    ShowBanner = s.Banner,
                    ShowDesc = s.Description,
                    ShowStatus = s.Status,
                    EntranceFee = s.EntranceFee
                }).ToListAsync();

            var firstShow = shows.FirstOrDefault();

            if (firstShow == null)
            {
                throw new Exception("No show found");
            }

            if (firstShow.ShowStatus.Equals("finished", StringComparison.OrdinalIgnoreCase))
            {
                var groups  = _context.Groups
                   .Where(g => g.ShowId == firstShow.ShowId && g.Status == true)
                   .Select(g => new GroupModel
                   {
                       GroupId = g.Id,
                       GroupName = g.Name,
                       Registrations = g.Registrations
                           .Where(r => r.Rank == 1 || r.Rank == 2 || r.Rank == 3 || r.IsBestVote == true)
                           .OrderBy(r=> r.Rank)
                           .Select(r => new RegistrationModel
                           {
                               Id = r.Id,
                               Name = r.Koi!.Name,
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
                        RefereeName = r.User!.Name
                    }).ToList();
            }
            return shows;
        }

        public async Task<int> AddNewShow(ShowDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("ShowDTO is null");
            }
            if (dto.RegisterStartDate > dto.RegisterEndDate)
            {
                throw new ArgumentException("Start date cannot be greater than end date");
            }

            if (dto.ScoreStartDate > dto.ScoreEndDate)
            {
                throw new ArgumentException("Start date cannot be greater than end date");
            }
            if(dto.EntranceFee == null || dto.EntranceFee <= 0)
            {
                throw new ArgumentException("Entrance fee can not null or negative");
            }
            int result = 0;
            Show show = new()
            {
                Title = dto.Title!,
                Description = dto.Description,
                ScoreStartDate = dto.ScoreStartDate,
                RegisterStartDate = dto.RegisterStartDate,
                RegisterEndDate = dto.RegisterEndDate,
                ScoreEndDate = dto.ScoreEndDate,
                Banner = await _s3Service.UploadShowBannerImage(dto.Banner!),
                Status = "up comming",
                //Groups = dto.Groups!.Select(g => new Group
                //{
                //    Name = g.Name,
                //    SizeMin = g.MinSize,
                //    SizeMax = g.MaxSize,
                //    Varieties = _context.Varieties.Where(v => g.Varieties!.Contains(v.Id)).ToList(),
                //    Criteria = g.Criterias!.Select(c => new Criterion
                //    {
                //        Name = c.Name,
                //        Percentage = c.Percentage,
                //        Description = c.Description,
                //        Status = true,
                //    }).ToList()
                //}).ToList(),
                EntranceFee = dto.EntranceFee,
            };
            
            _context.Shows.Add(show);                 
            result = await _context.SaveChangesAsync();
            return result;
        }



        public async Task<bool> ChangeShowStatus(string status, int showId)
        {
            string[] validStatus = {"up comming", "on going", "finished", "scoring"};

            if (!validStatus.Contains(status.ToLower()))
            {
                throw new ArgumentException("Invalid status");
            }
            var show = _context.Shows.Find(showId);
            if (show == null)
            {
                throw new Exception("Show not found");
            }
            show.Status = status;
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        //để tạm ở đây
        public async Task<bool> UpdateRegistrationAverageScore(int registrationId)
        {

            var registration =_context.Registrations
                                         .Include(r => r.Scores)
                                         .FirstOrDefault(r => r.Id == registrationId);

            if (registration == null)
            {
                throw new ArgumentException($"Registration with Id {registrationId} not found.");
                
            }
            //lấy đơn đó có bao nhiêu điểm
            var scoreCount = registration.Scores.Count;
            //Nếu scoreCount đó lớn hơn không thì tính trung bình cộng
            decimal averageScore = scoreCount > 0 ? registration.Scores.Average(score => score.Score1 ?? 0) : 0;

            registration.TotalScore = averageScore;
            registration.Status = "scored";

            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public Task<List<ShowModel>> GetAllShow()
        {
            return _context.Shows.Select(s => new ShowModel
            {
                ShowId = s.Id,
                ShowTitle = s.Title,
                ShowBanner = s.Banner,
                ShowDesc = s.Description,
                ShowStatus = s.Status
            }).ToListAsync();
        }

        public async Task<bool> UpdateAShow(ShowDTO dto)
        {
            var show = await _context.Shows.FindAsync(dto.Id);

            if (show == null)
            {
                throw new Exception("Show not found");
            }
            if (show.Status.ToLower() == "on going")
            {
                throw new Exception("Show is on going");
            }

            if (dto.RegisterStartDate > dto.RegisterEndDate)
            {
                throw new ArgumentException("Start date cannot be greater than end date");
            }

            if (dto.ScoreStartDate > dto.ScoreEndDate)
            {
                throw new ArgumentException("Start date cannot be greater than end date");
            }

            if (dto.Title != null)
            {
                show.Title = dto.Title!;
            }
            if (dto.Description != null)
            {
                show.Description = dto.Description!;
            }

            if (dto.Banner != null)
            {
                show.Banner = await _s3Service.UpdateImageAsync(show.Banner!, dto.Banner!);
            }
            if (dto.ScoreStartDate != null)
            {
                show.ScoreStartDate = (DateOnly)dto.ScoreStartDate!;
            }
            if (dto.RegisterStartDate != null)
            {
                show.RegisterStartDate = (DateOnly)dto.RegisterStartDate!;
            }
            if (dto.RegisterEndDate != null)
            {
                show.RegisterEndDate = (DateOnly)dto.RegisterEndDate!;
            }
            if (dto.ScoreEndDate != null)
            {
                show.ScoreEndDate = (DateOnly)dto.ScoreEndDate!;
            }
            if (dto.EntranceFee != null && dto.EntranceFee > 0)
            {
                show.EntranceFee = dto.EntranceFee;
            }
            int result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return false;
            }
            return true;
        }


        public async Task<List<DashboardRegistrationModel>> CountRegistrationsOfRecentShowAsync(int quantityOfShow)
        {
            List<DashboardRegistrationModel> result = new List<DashboardRegistrationModel>();
            // Lấy n Show cần:
            var shows = await _context.Shows
                .Where(s => s.Status!.Equals("Finished"))
                .Include(s => s.Registrations)
                .OrderByDescending(s => s.RegisterStartDate)
                .Take(quantityOfShow).ToListAsync();
            // Tính total registration:
            foreach (var show in shows)
            {
                int totalRegistration = 0;
                foreach (var registration in show.Registrations)
                {
                    totalRegistration++;
                }
                result.Add(new DashboardRegistrationModel()
                {
                    ShowId = show.Id,
                    ShowTitle = show.Title,
                    StartRegisterDate = show.RegisterStartDate,
                    Status = show.Status,
                    TotalRegistrations = totalRegistration,
                });
            };
            return result.OrderBy(r => r.StartRegisterDate).ToList();
        }

        public async Task<List<DashboardRevenueModel>> CalculateRevenueOfRecentShowAsync(int quantityOfShow)
        {
            List<DashboardRevenueModel> result = new List<DashboardRevenueModel>();
            // Lấy n Show cần:
            var shows = await _context.Shows
                .Where(s => s.Status!.Equals("Finished"))
                .Include(s => s.Registrations)
                .OrderByDescending(s => s.RegisterStartDate)
                .Take(quantityOfShow).ToListAsync();
            // Tính total registration:
            foreach (var show in shows)
            {
                int totalRegistration = 0;
                foreach (var registration in show.Registrations)
                {
                    totalRegistration++;
                }
                result.Add(new DashboardRevenueModel()
                {
                    ShowId = show.Id,
                    ShowTitle = show.Title,
                    StartRegisterDate = show.RegisterStartDate,
                    Status = show.Status,
                    TotalRevenue = (decimal) show.EntranceFee! * totalRegistration,
                });
            };
            return result.OrderBy(r => r.StartRegisterDate).ToList();
        }

        public async Task<List<DashboardVarietyModel>> CountKoiVarietyOfRecentShowAsync(int quantityOfShow)
        {
            List<DashboardVarietyModel> result = new List<DashboardVarietyModel>();
            // Lấy n Show cần:
            var shows = await _context.Shows
                .Where(s => s.Status!.Equals("Finished"))
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Koi)
                        .ThenInclude(k => k!.Variety)
                .OrderByDescending(s => s.RegisterStartDate)
                .Take(quantityOfShow).ToListAsync();
            foreach (var show in shows)
            {
                foreach (var registration in show.Registrations)
                {
                    var variety = result.SingleOrDefault(v => v.VarietyId == registration.Koi!.VarietyId);
                    // Kiểm tra nếu list variety chưa có loài này:
                    if (variety == null)
                    {
                        result.Add(new DashboardVarietyModel()
                        {
                            VarietyId = registration.Koi!.Variety.Id,
                            VarietyName = registration.Koi!.Variety.Name,
                            Quantity = 1
                        });
                    }
                    // Nếu đã có rồi:
                    else
                    {
                        variety.Quantity += 1;
                    }
                }
            };
            return result.OrderByDescending(v => v.Quantity).ToList();
        }

        // implement method delete a show by showId , delete group, criterion, refereeDetail , groupDetail
        public async Task<bool> DeleteShowAsync(int showId)
        {
            var show = await _context.Shows
                            .Include(s => s.Groups)
                                .ThenInclude(g => g.Criteria)
                            .Include(s => s.Groups)
                                .ThenInclude(g => g.Varieties)
                            .Include(s => s.RefereeDetails).SingleAsync(s => s.Id == showId);

            if (show.Status!.ToLower() != "up comming")
            {
                throw new Exception("This is not an up comming show");
            }
            
            if (!show.Groups.IsNullOrEmpty())
            {
                // delete all criteria and varieties of all groups
                foreach (var group in show.Groups)
                {
                    _context.Criteria.RemoveRange(group.Criteria);
                    _context.Varieties.RemoveRange(group.Varieties);
                }
                // delete all groups
                _context.Groups.RemoveRange(show.Groups);
            }

            // delete all refereeDetails
            _context.RefereeDetails.RemoveRange(show.RefereeDetails);

            _context.Shows.Remove(show);
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }


        //public async Task<bool> EditAShow(ShowDTO dto)
        //{
        //    var show = await _context.Shows.FindAsync(dto.Id);

        //    if (show == null)
        //    {
        //        throw new Exception("Show not found");
        //    }
        //    if (show.Status == "on going")
        //    {
        //        throw new Exception("Show is on going");
        //    }

        //    if (dto.RegisterStartDate > dto.RegisterEndDate)
        //    {
        //        throw new ArgumentException("Start date cannot be greater than end date");
        //    }

        //    if (dto.ScoreStartDate > dto.ScoreEndDate)
        //    {
        //        throw new ArgumentException("Start date cannot be greater than end date");
        //    }

        //    if (show.Title != dto.Title)
        //    {
        //        show.Title = dto.Title;
        //    }
        //    show.Description = dto.Description;
        //    show.ScoreStartDate = dto.ScoreStartDate;
        //    show.RegisterStartDate = dto.RegisterStartDate;
        //    show.RegisterEndDate = dto.RegisterEndDate;
        //    show.ScoreEndDate = dto.ScoreEndDate;


        //    var newGroups = dto.Groups.Select(g => new Group
        //    {
        //        Name = g.Name,
        //        SizeMin = g.MinSize,
        //        SizeMax = g.MaxSize,
        //        Varieties = _context.Varieties.Where(v => g.Varieties.Contains(v.Id)).ToList(),
        //        Criteria = g.Criterias.Select(c => new Criterion
        //        {
        //            Name = c.Name,
        //            Percentage = c.Percentage,
        //            Description = c.Description,
        //            Status = true
        //        }).ToList()
        //    }).ToList();

        //    _context.Groups.AddRange(newGroups);

        //    // Remove old groups
        //    var oldGroups = _context.Groups.Where(g => g.ShowId == show.Id && !newGroups.Any(ng => ng.Id == g.Id)).ToList();
        //    _context.Groups.RemoveRange(oldGroups);

        //    // Update existing groups
        //    foreach (var group in show.Groups)
        //    {
        //        var updatedGroup = dto.Groups.FirstOrDefault(g => g.Id == group.Id);
        //        if (updatedGroup != null)
        //        {
        //            group.Name = updatedGroup.Name;
        //            group.SizeMin = updatedGroup.MinSize;
        //            group.SizeMax = updatedGroup.MaxSize;
        //            group.Varieties = _context.Varieties.Where(v => updatedGroup.Varieties.Contains(v.Id)).ToList();

        //            // Add new criteria
        //            var newCriterias = updatedGroup.Criterias.Where(c => !group.Criteria.Any(gc => gc.Id == c.Id)).Select(c => new Criterion
        //            {
        //                Name = c.Name,
        //                Percentage = c.Percentage,
        //                Description = c.Description,
        //                Status = true
        //            }).ToList();
        //            group.Criteria.AddRange(newCriterias);

        //            // Remove old criteria
        //            var oldCriterias = group.Criteria.Where(c => !updatedGroup.Criterias.Any(uc => uc.Id == c.Id)).ToList();
        //            group.Criteria.RemoveAll(c => oldCriterias.Contains(c));
        //        }
        //    }

        //    await _context.SaveChangesAsync();

        //    return true;
        //}



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