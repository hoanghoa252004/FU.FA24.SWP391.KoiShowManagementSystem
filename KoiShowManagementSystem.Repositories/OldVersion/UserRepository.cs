//using KoiShowManagementSystem.Entities;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Runtime.CompilerServices;
//using System.Security.Claims;
//using System.Text;

//namespace KoiShowManagementSystem.Repositories.OldVersion
//{
//    public class UserRepository : IUserRepository
//    {
//        // Fields:
//        private readonly KoiShowManagementSystemContext _context;
//        private readonly IConfiguration _configuration;
//        private readonly JwtServices _jwtServices;

//        private readonly bool DEFAULT_USER_STATUS = true;
//        private readonly bool VALID_STATUS = true;

//        // CONSTRUCTOR:
//        public UserRepository(KoiShowManagementSystemContext context, IConfiguration configuration, JwtServices jwtServices)
//        {
//            _context = context;
//            _configuration = configuration;
//            _jwtServices = jwtServices;
//        }

//        //1. CREATE USER: ----------------------------------------------------------------------------
//        public async Task<bool> CreateUser(SignUpModel dto)
//        {
//            bool result = false;
//            try
//            {
//                // Get Role ID ( Member ):
//                var role = await _context.Roles.
//                    SingleOrDefaultAsync(role => role.Title == "Member" && role.Status == VALID_STATUS);
//                // Create User ( Member ):
//                await _context.AddAsync(new User()
//                {
//                    Email = dto.Email!,
//                    Password = dto.Password!,
//                    Phone = dto.Phone!,
//                    Name = dto.Name!,
//                    RoleId = role!.Id,
//                    Gender = dto.Gender,
//                    DateOfBirth = dto.DateOfBirth,
//                    Status = DEFAULT_USER_STATUS,
//                });
//                await _context.SaveChangesAsync();
//                result = true;
//            }
//            catch (DbUpdateException ex)
//            {
//                if (ex.InnerException is SqlException sqlEx)
//                {
//                    if (sqlEx!.Number == 2627
//                        || sqlEx.Number == 2601
//                        && sqlEx.Message.Contains("UQ__User__Email"))
//                    {
//                        throw new Exception("Email has already existed");
//                    }
//                }
//            }
//            return result;
//        }


//        // 2. GET USER: -----------------------------------------------------------------------------
//        public async Task<UserDTO> GetUser(LoginModel dto)
//        {
//            UserDTO result = null!;
//            // Get User:
//            var user = await _context.Users.
//            SingleOrDefaultAsync(user => user.Email == dto.Email && user.Password == dto.Password);
//            // Check Password:
//            if (user != null && user.Status == VALID_STATUS)
//            {
//                bool chkPassword = string.Equals(user.Password, dto.Password, StringComparison.Ordinal);
//                if (chkPassword == true)
//                {
//                    // Get Role Title ( Member ):
//                    var role = await _context.Roles.
//                        SingleOrDefaultAsync(role => role.Id == user!.RoleId && role.Status == VALID_STATUS);
//                    result = new UserDTO
//                    {
//                        Id = user!.Id,
//                        Name = user!.Name,
//                        Email = user.Email,
//                        Role = role!.Title,
//                        Phone = user!.Phone,
//                        Token = _jwtServices.GenerateAccessToken(user.Email, user.Name, user.Id, role.Title)
//                    };
//                }
//            }
//            else if (user != null && user.Status != VALID_STATUS)
//                //return "Your account is banned";
//                throw new Exception("Your account is banned");
//            return result;
//        }


//        // 3. GET PERSONAL INFORMATION: -------------------------------------------------------------
//        public async Task<ProfileDTO> GetPersonalInfo()
//        {
//            ProfileDTO? result = null;
//            // Get User_Id from Access Token:
//            int id = _jwtServices.GetIdAndRoleFromToken().userId; // Nếu lấy ra không được sẽ quăng exception.

//            // Get user:
//            var user = await _context.Users.SingleOrDefaultAsync(user => user.Id == id);
//            if (user == null)
//                throw new Exception("Invalid User");

//            // Get role name:
//            var role = await _context.Roles.SingleOrDefaultAsync(role => role.Id == user.RoleId);
//            // Lấy email, password, name, phone, role
//            result = new ProfileDTO()
//            {
//                Id = user.Id,
//                Name = user.Name,
//                Email = user.Email,
//                Gender = user.Gender,
//                DateOfBirth = user.DateOfBirth,
//                Phone = user.Phone,
//                Role = role!.Title,
//            };
//            return result!;
//        }


//        // 4. CHANGE PERSONAL INFORMATION: -----------------------------------------------------------
//        public async Task<bool> EditPersonalInfo(EditedProfileModel dto)
//        {
//            //  Edit name, phone number, gender, date of birth ( not  EMAIL ).
//            bool result = false;

//            // Get User_Id from Access Token:
//            int id = _jwtServices.GetIdAndRoleFromToken().userId; // Nếu lấy ra không được sẽ quăng exception.

//            // Get User:
//            var user = await _context.Users.SingleOrDefaultAsync(user => user.Id == id);
//            if (user != null)
//            {
//                // Update:
//                user.Name = dto.Name!;
//                user.Phone = dto.Phone!;
//                user.Gender = dto.Gender!;
//                user.DateOfBirth = dto.DateOfBirth!;
//                // Save Change:
//                await _context.SaveChangesAsync();
//                result = true;
//            }
//            return result;
//        }


//        // 5. CHANGE PASSWORD: ---------------------------------------------------------------------
//        public async Task<bool> UpdatePassword(ChangingPasswordModel dto)
//        {
//            bool result = false;
//            // Get User_Id from Access Token:
//            int id = _jwtServices.GetIdAndRoleFromToken().userId; // Nếu lấy ra không được sẽ quăng exception.
//            // Get User:
//            var user = await _context.Users.SingleOrDefaultAsync(user => user.Id == id && user.Password == dto.CurentPassword);
//            if (user != null)
//            {
//                // Check Password:
//                bool chkPassword = string.Equals(user.Password, dto.CurentPassword, StringComparison.Ordinal);
//                if (chkPassword == true)
//                {
//                    // Update:
//                    user.Password = dto.NewPassword!;
//                    // Save Change:
//                    await _context.SaveChangesAsync();
//                    result = true;
//                }
//            }
//            return result;
//        }


//        // 6. GET KOI REGISTRATIONS: ---------------------------------------------------------------
//        public async Task<IEnumerable<KoiRegistrationDTO>> GetInProcessKoiRegistration(string status)
//        {
//            IEnumerable<KoiRegistrationDTO> result = null!;
//            // Get User_Id from Access Token:
//            int id = _jwtServices.GetIdAndRoleFromToken().userId; // Nếu lấy ra không được sẽ quăng exception.
//            status = status.ToLower();
//            switch (status)
//            {
//                case "inprocess":
//                    {
//                        result = await (from illustration in _context.Illustrations
//                                        join koiReg in _context.KoiRegistrations on illustration.KoiId equals koiReg.Id
//                                        where koiReg.UserId == id
//                                        && (koiReg.Status!.Contains("Pending")
//                                        || koiReg.Status!.Contains("Accepted")
//                                        || koiReg.Status!.Contains("Reject"))
//                                        join variety in _context.Varieties on koiReg.VarietyId equals variety.Id
//                                        join grp in _context.Groups on koiReg.GroupId equals grp.Id
//                                        join show in _context.Shows on grp.ShowId equals show.Id
//                                        select new KoiRegistrationDTO
//                                        {
//                                            Id = koiReg.Id,
//                                            KoiName = koiReg.Name,
//                                            Description = koiReg.Description,
//                                            Size = koiReg.Size,
//                                            Variety = variety.Name,
//                                            Show = show.Title,
//                                            Group = grp.Name,
//                                            Status = koiReg.Status,
//                                            TotalScore = koiReg.TotalScore,
//                                            Image = illustration.ImageUrl,
//                                            Video = illustration.VideoUrl
//                                        }).ToListAsync();
//                        break;
//                    }
//                case "scored":
//                    {
//                        result = await (from illustration in _context.Illustrations
//                                        join koiReg in _context.KoiRegistrations on illustration.KoiId equals koiReg.Id
//                                        join variety in _context.Varieties on koiReg.VarietyId equals variety.Id
//                                        join grp in _context.Groups on koiReg.GroupId equals grp.Id
//                                        join show in _context.Shows on grp.ShowId equals show.Id
//                                        where koiReg.UserId == id && koiReg.Status!.Contains("Scored")
//                                        select new KoiRegistrationDTO
//                                        {
//                                            Id = koiReg.Id,
//                                            KoiName = koiReg.Name,
//                                            Description = koiReg.Description,
//                                            Size = koiReg.Size,
//                                            Variety = variety.Name,
//                                            Show = show.Title,
//                                            Group = grp.Name,
//                                            Status = koiReg.Status,
//                                            TotalScore = koiReg.TotalScore,
//                                            Image = illustration.ImageUrl,
//                                            Video = illustration.VideoUrl
//                                        }).ToListAsync();
//                        break;
//                    }
//                case "draft":
//                    {
//                        result = await (from illustration in _context.Illustrations
//                                        join koiReg in _context.KoiRegistrations on illustration.KoiId equals koiReg.Id
//                                        join variety in _context.Varieties on koiReg.VarietyId equals variety.Id
//                                        join grp in _context.Groups on koiReg.GroupId equals grp.Id
//                                        join show in _context.Shows on grp.ShowId equals show.Id
//                                        where koiReg.UserId == id && koiReg.Status!.Contains("Draft")
//                                        select new KoiRegistrationDTO
//                                        {
//                                            Id = koiReg.Id,
//                                            KoiName = koiReg.Name,
//                                            Description = koiReg.Description,
//                                            Size = koiReg.Size,
//                                            Variety = variety.Name,
//                                            Show = show.Title,
//                                            Group = grp.Name,
//                                            Status = koiReg.Status,
//                                            TotalScore = koiReg.TotalScore,
//                                            Image = illustration.ImageUrl,
//                                            Video = illustration.VideoUrl
//                                        }).ToListAsync();
//                        break;
//                    }
//            }
//            return result;
//        }
//    }
//}
