//using KoiShowManagementSystem.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Identity.Client;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

//namespace KoiShowManagementSystem.Repositories.OldVersion
//{
//    public class KoiShowRepository : IKoiShowRepository
//    {
//        //Config connect DbContext
//        private readonly KoiShowManagementSystemContext _context;
//        public KoiShowRepository(KoiShowManagementSystemContext context)
//        {
//            _context = context;
//        }

//        public Task<Show> CreateShow()
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<IEnumerable<KoiDetailDTO>> GetKoiByShowId(int pageIndex, int pageSize, int showId)
//        {
//            var kois = _context.KoiRegistrations
//                .Include(k => k.Group)
//                .Include(k => k.Variety)
//                .Include(k => k.Illustration)
//                .Where(k => k.Group.ShowId == showId)
//                .Select(k => new KoiDetailDTO
//                {
//                    Id = k.Id,
//                    Name = k.Name,
//                    Size = k.Size,
//                    VarietyName = k.Variety!.Name,
//                    image = k.Illustration!.ImageUrl
//                });
//            var koi = await kois
//                .Skip((pageIndex - 1) * pageSize)
//                .Take(pageSize)
//                .ToListAsync();

//            return koi;
//        }

//        public async Task<KoiDetailDTO> GetKoiDetail(int koiId)
//        {
//            var koiDetail = await (from kr in _context.KoiRegistrations
//                                   join u in _context.Users on kr.UserId equals u.Id
//                                   join g in _context.Groups on 1 equals 1
//                                   join s in _context.Shows on g.ShowId equals s.Id
//                                   where kr.Id == koiId && kr.Size >= g.SizeMin && kr.Size <= g.SizeMax
//                                   select new KoiDetailDTO
//                                   {
//                                       Id = kr.Id,
//                                       Name = kr.Name,
//                                       UserName = u.Name,
//                                       GroupName = g.Name,
//                                       ShowTitle = s.Title,
//                                       TotalScore = kr.TotalScore,
//                                       //RegistrationStatus = (s.RegisterStartDate <= DateTime.Now && s.RegisterEndDate >= DateTime.Now)
//                                       //                     ? "true" : "false"
//                                   }).FirstOrDefaultAsync();

//            if (koiDetail != null)
//            {
//                // Lấy số lượng vote
//                //koiDetail.VoteCount = await _context.Votes.CountAsync(v => v.KoiId == koiId);
//            }

//            return koiDetail!;
//        }

//        public Task<Show> GetShowById(int showId)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<KoiShowDTO> GetShowDetails(int showId)
//        {
//            if (showId > 0)
//            {
//                var showDetails = await (from s in _context.Shows
//                                         where s.Id == showId
//                                         select new KoiShowDTO
//                                         {
//                                             ShowId = s.Id,
//                                             ShowTitle = s.Title,
//                                             Description = s.Description!,
//                                             Banner = s.Banner,
//                                             GroupName = (from b in _context.Groups
//                                                          where b.ShowId == s.Id
//                                                          select b.Name!).ToList(),
//                                             RefereeName = (from r in _context.RefereeDetails
//                                                            join u in _context.Users on r.UserId equals u.Id
//                                                            where r.ShowId == s.Id
//                                                            select u.Name!).ToList(),
//                                             Status = s.Status == "Up Comming" ? 0 : s.Status == "On Going" ? 1 : s.Status == "Finished" ? 2 : -1
//                                         }).FirstOrDefaultAsync();

//                return showDetails!;
//            }
//            return null!;

//        }

//        public async Task<(int TotalItems, List<Show> Shows)> SearchShow(int pageIndex, int pageSize, string keyword)
//        {
//            var query = _context.Shows.AsQueryable();
//            if (!string.IsNullOrWhiteSpace(keyword))
//            {
//                query = query.Where(s => s.Title.Contains(keyword));
//            }
//            var totalItems = await query.CountAsync();
//            var shows = await query
//                .Skip((pageIndex - 1) * pageSize)
//                .Take(pageSize)
//                .ToListAsync();
//            return (totalItems, shows);
//        }


//        public async Task<object?> GetClosestShow()
//        {
//            object result;
//            var closestShow = await _context.Shows
//                                    .Include(s => s.Groups)
//                                    .OrderByDescending(s => s.StartDate)
//                                    .FirstOrDefaultAsync();
//            if (closestShow == null)
//            {
//                throw new ArgumentNullException(nameof(closestShow), "No closest show found.");
//            }
//            if (closestShow.Status.Equals("Finished"))
//            {
//                var groupIds = closestShow.Groups.Select(g => g.Id).ToList();

//                var topKoiRegistrations = await _context.KoiRegistrations
//                                            .Where(kr => groupIds.Contains(kr.GroupId) &&
//                                            (kr.Rank == 1 || kr.Rank == 2 || kr.Rank == 3 || kr.IsBestVote == true))
//                                            .GroupBy(kr => kr.GroupId)
//                                            .ToListAsync();

//                foreach (var group in closestShow.Groups)
//                {
//                    var groupKoiRegistrations = topKoiRegistrations
//                        .Where(gr => gr.Key == group.Id)
//                        .SelectMany(gr => gr)
//                        .OrderByDescending(kr => kr.Rank)
//                        .ToList();

//                    group.KoiRegistrations = groupKoiRegistrations;
//                }

//                return new
//                {
//                    ShowId = closestShow.Id,
//                    closestShow.Title,
//                    Status = closestShow.Status.Equals("Finished", StringComparison.OrdinalIgnoreCase) ||
//                             closestShow.Status.Equals("Up Coming", StringComparison.OrdinalIgnoreCase)
//                             ? 1
//                             : closestShow.Status.Equals("On Going", StringComparison.OrdinalIgnoreCase)
//                             ? 2
//                             : 0,
//                    closestShow.StartDate,
//                    Groups = closestShow.Groups.Select(g => new
//                    {
//                        GroupId = g.Id,
//                        GroupName = g.Name,
//                        KoiRegistrations = g.KoiRegistrations.Select(kr => new
//                        {
//                            KoiId = kr.Id,
//                            KoiName = kr.Name,
//                            kr.Rank,
//                            kr.IsBestVote
//                        }).ToList()
//                    }).ToList()
//                };
//            }
//            if (closestShow.Status.Equals("Up comming") || closestShow.Status.Equals("On going"))
//            {
//                //get the referee for the show
//                var refereeDetails = await _context.RefereeDetails
//                                    .Include(rd => rd.User)
//                                    .Where(rd => rd.ShowId == closestShow.Id)
//                                    .ToListAsync();
//                closestShow.RefereeDetails = refereeDetails;
//                return new
//                {
//                    ShowId = closestShow.Id,
//                    closestShow.Title,
//                    closestShow.Status,
//                    closestShow.StartDate,
//                    Referees = refereeDetails.Select(rd => new
//                    {
//                        RefereeName = rd.User.Name
//                    }).ToList(),
//                };
//            }

//            return null;
//        }

//        public async Task<object?> GetPagingShow(int page)
//        {
//            int pageSize = 4;
//            int skipCount = (page - 1) * pageSize;

//            var result = await _context.Shows
//                  .OrderByDescending(s => s.StartDate)
//                  .Skip(1)
//                  .Skip(skipCount)
//                  .Take(pageSize)
//                  .Select(s => new
//                  {
//                      s.Id,
//                      s.Title,
//                      s.Banner,
//                  })
//                  .ToListAsync();
//            return result;
//        }
//    }
//}
