using CompanyA.BusinessEntity;
using CompanyA.DataAccess;
using CompanyA.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyA.BusinessComponents
{
    public interface IPersonnelService
    {
        Task<IEnumerable<PersonnelDto>> GetAllPersonnelAsync();
        Task<PersonnelDto?> GetPersonnelByIdAsync(int id);
        Task<PersonnelDto> CreatePersonnelAsync(PersonnelDto personnelDto);
        Task<PersonnelDto?> UpdatePersonnelAsync(int id, PersonnelDto personnelDto);
        Task<bool> DeletePersonnelAsync(int id, bool confirm = false);
        Task<bool> PersonnelExistsAsync(int id);
        Task<bool> CommissionProfileExistsAsync(int profileId);
    }

    public class PersonnelService : IPersonnelService
    {
        private readonly MarketingDbContext _context;

        public PersonnelService(MarketingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PersonnelDto>> GetAllPersonnelAsync()
        {
            var personnel = await _context.Personnel
                .Include(p => p.CommissionProfile)
                .ToListAsync();

            return personnel.Select(MapToDto);
        }

        public async Task<PersonnelDto?> GetPersonnelByIdAsync(int id)
        {
            var personnel = await _context.Personnel
                .Include(p => p.CommissionProfile)
                .FirstOrDefaultAsync(p => p.Id == id);

            return personnel != null ? MapToDto(personnel) : null;
        }

        public async Task<PersonnelDto> CreatePersonnelAsync(PersonnelDto personnelDto)
        {
            var personnel = MapToEntity(personnelDto);
            
            _context.Personnel.Add(personnel);
            await _context.SaveChangesAsync();

            return await GetPersonnelByIdAsync(personnel.Id) ?? personnelDto;
        }

        public async Task<PersonnelDto?> UpdatePersonnelAsync(int id, PersonnelDto personnelDto)
        {
            var existingPersonnel = await _context.Personnel.FindAsync(id);
            if (existingPersonnel == null)
                return null;

            existingPersonnel.Name = personnelDto.Name.Trim();
            existingPersonnel.Age = personnelDto.Age;
            existingPersonnel.Phone = personnelDto.Phone.Trim();
            existingPersonnel.CommissionProfileId = personnelDto.CommissionProfileId;
            existingPersonnel.BankName = string.IsNullOrWhiteSpace(personnelDto.BankName) ? null : personnelDto.BankName.Trim();
            existingPersonnel.BankAccountNo = string.IsNullOrWhiteSpace(personnelDto.BankAccountNo) ? null : personnelDto.BankAccountNo.Trim();

            await _context.SaveChangesAsync();

            return await GetPersonnelByIdAsync(id);
        }

        public async Task<bool> DeletePersonnelAsync(int id, bool confirm = false)
        {
            if (!confirm)
                return false;

            var personnel = await _context.Personnel.FindAsync(id);
            if (personnel == null)
                return false;

            _context.Personnel.Remove(personnel);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> PersonnelExistsAsync(int id)
        {
            return await _context.Personnel.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> CommissionProfileExistsAsync(int profileId)
        {
            return await _context.CommissionProfiles.AnyAsync(cp => cp.Id == profileId);
        }

        private static PersonnelDto MapToDto(Personnel personnel)
        {
            return new PersonnelDto
            {
                Id = personnel.Id,
                Name = personnel.Name,
                Age = personnel.Age,
                Phone = personnel.Phone,
                CommissionProfileId = personnel.CommissionProfileId,
                BankName = personnel.BankName,
                BankAccountNo = personnel.BankAccountNo
            };
        }

        private static Personnel MapToEntity(PersonnelDto dto)
        {
            return new Personnel
            {
                Name = dto.Name.Trim(),
                Age = dto.Age,
                Phone = dto.Phone.Trim(),
                CommissionProfileId = dto.CommissionProfileId,
                BankName = string.IsNullOrWhiteSpace(dto.BankName) ? null : dto.BankName.Trim(),
                BankAccountNo = string.IsNullOrWhiteSpace(dto.BankAccountNo) ? null : dto.BankAccountNo.Trim()
            };
        }
    }

    // Sales Service Interface
    public interface ISalesService
    {
        Task<IEnumerable<SalesDto>> GetSalesAsync(int? personnelId = null, DateTime? from = null, DateTime? to = null);
        Task<SalesDto?> GetSalesByIdAsync(int id);
        Task<SalesDto> CreateSalesAsync(SalesDto salesDto);
        Task<bool> DeleteSalesAsync(int id);
        Task<bool> SalesExistsAsync(int id);
    }

    // Sales Service Implementation
    public class SalesService : ISalesService
    {
        private readonly MarketingDbContext _context;
        private readonly ILogger<SalesService> _logger;

        public SalesService(MarketingDbContext context, ILogger<SalesService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<SalesDto>> GetSalesAsync(int? personnelId = null, DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Sales.AsQueryable();

            if (personnelId.HasValue)
            {
                query = query.Where(s => s.PersonnelId == personnelId.Value);
            }

            if (from.HasValue)
            {
                query = query.Where(s => s.ReportDate >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(s => s.ReportDate <= to.Value);
            }

            var sales = await query.OrderByDescending(s => s.ReportDate).ToListAsync();

            return sales.Select(s => new SalesDto
            {
                Id = s.Id,
                PersonnelId = s.PersonnelId,
                ReportDate = s.ReportDate,
                SalesAmount = s.SalesAmount
            });
        }

        public async Task<SalesDto?> GetSalesByIdAsync(int id)
        {
            var sales = await _context.Sales.FindAsync(id);
            if (sales == null) return null;

            return new SalesDto
            {
                Id = sales.Id,
                PersonnelId = sales.PersonnelId,
                ReportDate = sales.ReportDate,
                SalesAmount = sales.SalesAmount
            };
        }

        public async Task<SalesDto> CreateSalesAsync(SalesDto salesDto)
        {
            // Validate report date is not in the future
            if (salesDto.ReportDate > DateTime.Today)
            {
                throw new ArgumentException("Report date cannot be in the future");
            }

            // Verify personnel exists
            var personnelExists = await _context.Personnel.AnyAsync(p => p.Id == salesDto.PersonnelId);
            if (!personnelExists)
            {
                throw new ArgumentException($"Personnel with ID {salesDto.PersonnelId} does not exist");
            }

            var sales = new Sales
            {
                PersonnelId = salesDto.PersonnelId,
                ReportDate = salesDto.ReportDate,
                SalesAmount = salesDto.SalesAmount
            };

            _context.Sales.Add(sales);
            await _context.SaveChangesAsync();

            salesDto.Id = sales.Id;
            return salesDto;
        }

        public async Task<bool> DeleteSalesAsync(int id)
        {
            var sales = await _context.Sales.FindAsync(id);
            if (sales == null) return false;

            _context.Sales.Remove(sales);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SalesExistsAsync(int id)
        {
            return await _context.Sales.AnyAsync(s => s.Id == id);
        }
    }

    // CommissionProfile Service Interface
    public interface ICommissionProfileService
    {
        Task<IEnumerable<CommissionProfileDto>> GetAllCommissionProfilesAsync();
        Task<CommissionProfileDto?> GetCommissionProfileByIdAsync(int id);
        Task<CommissionProfileDto> CreateCommissionProfileAsync(CommissionProfileDto profileDto);
        Task<CommissionProfileDto?> UpdateCommissionProfileAsync(int id, CommissionProfileDto profileDto);
        Task<bool> DeleteCommissionProfileAsync(int id);
        Task<bool> CommissionProfileExistsAsync(int id);
        Task<bool> HasPersonnelReferencesAsync(int id);
    }

    // CommissionProfile Service Implementation
    public class CommissionProfileService : ICommissionProfileService
    {
        private readonly MarketingDbContext _context;
        private readonly ILogger<CommissionProfileService> _logger;

        public CommissionProfileService(MarketingDbContext context, ILogger<CommissionProfileService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CommissionProfileDto>> GetAllCommissionProfilesAsync()
        {
            var profiles = await _context.CommissionProfiles.OrderBy(p => p.ProfileName).ToListAsync();

            return profiles.Select(p => new CommissionProfileDto
            {
                Id = p.Id,
                ProfileName = p.ProfileName,
                CommissionFixed = p.CommissionFixed,
                CommissionPercentage = p.CommissionPercentage
            });
        }

        public async Task<CommissionProfileDto?> GetCommissionProfileByIdAsync(int id)
        {
            var profile = await _context.CommissionProfiles.FindAsync(id);
            if (profile == null) return null;

            return new CommissionProfileDto
            {
                Id = profile.Id,
                ProfileName = profile.ProfileName,
                CommissionFixed = profile.CommissionFixed,
                CommissionPercentage = profile.CommissionPercentage
            };
        }

        public async Task<CommissionProfileDto> CreateCommissionProfileAsync(CommissionProfileDto profileDto)
        {
            var profile = new CommissionProfile
            {
                ProfileName = profileDto.ProfileName,
                CommissionFixed = profileDto.CommissionFixed,
                CommissionPercentage = profileDto.CommissionPercentage
            };

            _context.CommissionProfiles.Add(profile);
            await _context.SaveChangesAsync();

            profileDto.Id = profile.Id;
            return profileDto;
        }

        public async Task<CommissionProfileDto?> UpdateCommissionProfileAsync(int id, CommissionProfileDto profileDto)
        {
            var profile = await _context.CommissionProfiles.FindAsync(id);
            if (profile == null) return null;

            profile.ProfileName = profileDto.ProfileName;
            profile.CommissionFixed = profileDto.CommissionFixed;
            profile.CommissionPercentage = profileDto.CommissionPercentage;

            await _context.SaveChangesAsync();

            return new CommissionProfileDto
            {
                Id = profile.Id,
                ProfileName = profile.ProfileName,
                CommissionFixed = profile.CommissionFixed,
                CommissionPercentage = profile.CommissionPercentage
            };
        }

        public async Task<bool> DeleteCommissionProfileAsync(int id)
        {
            // Check if any personnel reference this profile
            var hasReferences = await HasPersonnelReferencesAsync(id);
            if (hasReferences)
            {
                throw new InvalidOperationException("Cannot delete commission profile that is referenced by personnel records");
            }

            var profile = await _context.CommissionProfiles.FindAsync(id);
            if (profile == null) return false;

            _context.CommissionProfiles.Remove(profile);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CommissionProfileExistsAsync(int id)
        {
            return await _context.CommissionProfiles.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> HasPersonnelReferencesAsync(int id)
        {
            return await _context.Personnel.AnyAsync(p => p.CommissionProfileId == id);
        }
    }
}
