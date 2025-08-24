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
}
