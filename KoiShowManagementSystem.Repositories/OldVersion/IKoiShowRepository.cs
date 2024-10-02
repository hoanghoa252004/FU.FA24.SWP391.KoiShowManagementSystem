//using KoiShowManagementSystem.Entities;

//namespace KoiShowManagementSystem.Repositories.OldVersion
//{
//    public interface IKoiShowRepository
//    {
//        Task<Show> CreateShow();
//        Task<Show> GetShowById(int showId);
//        Task<KoiShowDTO> GetShowDetails(int showId);
//        Task<(int TotalItems, List<Show> Shows)> SearchShow(int pageIndex, int pageSize, string keyword);
//        Task<IEnumerable<KoiDetailDTO>> GetKoiByShowId(int pageIndex, int pageSize, int showId);
//        Task<KoiDetailDTO> GetKoiDetail(int koiId);
//        Task<object?> GetClosestShow();

//        Task<object?> GetPagingShow(int page);
//    }
//}
