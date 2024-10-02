using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public class KoiRegistrationService : IKoiRegistrationService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly JwtServices _jwtServices;
        public KoiRegistrationService(UnitOfWork unitOfWork, JwtServices jwtServices)
        {
            _unitOfWork = unitOfWork;
            _jwtServices = jwtServices;
        }

        // 1. GET KOI REGISTRATION BY MEMBER:
        public async Task<IEnumerable<object>> GetKoiRegistrationByUser(string status)
        {
            IEnumerable<object> result = null!;
            // 1.1 Get User ID from Token:
            // Nếu lấy ra không được sẽ quăng exception.
            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            // 1.2 Lấy User:
            var user = await _unitOfWork.Users.GetById(id);
            if (user != null)
            {
                var koiRegistrations = await _unitOfWork.KoiRegistrations.GetByUserID(id);
                var shows = await _unitOfWork.Shows.GetAll();
                var groups = await _unitOfWork.Groups.GetAll();
                var varieties = await _unitOfWork.Varieties.GetAll();
                var illustrations = await _unitOfWork.Illustrations.GetAll();
                // 1.3 Switch case các trường hợp mà Client cần lấy:
                status = status.ToLower();
                switch (status)
                {
                    case "inprocess":
                        {
                            result = from koi in koiRegistrations
                                     where koi.Status!.Contains("Pending")
                                        || koi.Status!.Contains("Accepted")
                                        || koi.Status!.Contains("Reject")
                                     join var in varieties on koi.VarietyId equals var.Id
                                     join ill in illustrations on koi.Id equals ill.KoiId
                                     join gro in groups on koi.GroupId equals gro.Id
                                     join sho in shows on gro.ShowId equals sho.Id
                                     select new
                                     {
                                         Id = koi.Id,
                                         KoiName = koi.Name,
                                         Description = koi.Description,
                                         Size = koi.Size,
                                         Variety = var.Name,
                                         ShowId = sho.Id,
                                         Show = sho.Title,
                                         Group = gro.Name,
                                         Status = koi.Status,
                                         TotalScore = koi.TotalScore,
                                         Image = ill.ImageUrl,
                                         Video = ill.VideoUrl
                                     };
                            break;
                        }
                    case "draft":
                        {
                            result = from koi in koiRegistrations
                                     where koi.Status!.Contains("Draft")
                                     join var in varieties on koi.VarietyId equals var.Id
                                     join ill in illustrations on koi.Id equals ill.KoiId
                                     join gro in groups on koi.GroupId equals gro.Id
                                     join sho in shows on gro.ShowId equals sho.Id
                                     select new
                                     {
                                         Id = koi.Id,
                                         KoiName = koi.Name,
                                         Description = koi.Description,
                                         Size = koi.Size,
                                         Variety = var.Name,
                                         ShowId = sho.Id,
                                         Show = sho.Title,
                                         Group = gro.Name,
                                         Status = koi.Status,
                                         TotalScore = koi.TotalScore,
                                         Image = ill.ImageUrl,
                                         Video = ill.VideoUrl
                                     };
                            break;
                        }
                    case "scored":
                        {
                            result = from koi in koiRegistrations
                                     where koi.Status!.Contains("Scored")
                                     join var in varieties on koi.VarietyId equals var.Id
                                     join ill in illustrations on koi.Id equals ill.KoiId
                                     join gro in groups on koi.GroupId equals gro.Id
                                     join sho in shows on gro.ShowId equals sho.Id
                                     select new
                                     {
                                         Id = koi.Id,
                                         KoiName = koi.Name,
                                         Description = koi.Description,
                                         Size = koi.Size,
                                         Variety = var.Name,
                                         ShowId = sho.Id,
                                         Show = sho.Title,
                                         Group = gro.Name,
                                         Status = koi.Status,
                                         TotalScore = koi.TotalScore,
                                         Image = ill.ImageUrl,
                                         Video = ill.VideoUrl
                                     };
                            break;
                        }
                }
            }
            return result;
        }
    }
}
