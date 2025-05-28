using Microsoft.EntityFrameworkCore;
using Streamling.Data;
using Streamling.Model.DTOs.Hostaway;
using Streamling.Model.Entities;

namespace Streamling.Repository
{
    public class ReservationRepository(StoreContext storeContext)
    {
        private readonly StoreContext _storeContext = storeContext;

        public async Task AddReservation(Reservation reservation)
        {
            _storeContext.Reservations.Add(reservation);
            await _storeContext.SaveChangesAsync();
        }

        public async Task<List<Reservation>> GetReservations()
        {
            return await _storeContext.Reservations.AsNoTracking().ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsByPlatformNameAndAccountId(string accountId, string platformName)
        {
            return await _storeContext.Reservations.Where(r => r.AccountId == accountId && r.PlatformName == platformName).ToListAsync();
        }


        public async Task AddReservations(List<Reservation> reservations)
        {
            _storeContext.Reservations.AddRange(reservations);
            await _storeContext.SaveChangesAsync();
        }

        public async Task UpdateReservation(Reservation reservation)
        {
            _storeContext.Reservations.Update(reservation);
            await _storeContext.SaveChangesAsync();
        }

        public async Task UpdateReservations(List<Reservation> reservations)
        {
            _storeContext.Reservations.UpdateRange(reservations);
            await _storeContext.SaveChangesAsync();
        }

        public async Task<Reservation?> GetReservation(ReceivingReservationDto hostawayReservationDto)
        {
            return await _storeContext.Reservations.FirstOrDefaultAsync(r => r.PlatformReservationId == hostawayReservationDto.ReservationId && r.PlatformPropertyId == hostawayReservationDto.PropertyId && r.AccountId == hostawayReservationDto.AccountId);
        }


        public async Task DeleteReservation(ReceivingReservationDto hostawayReservationDto)
        {
            var reservation = await GetReservation(hostawayReservationDto);
            if (reservation != null)
            {
                _storeContext.Reservations.Remove(reservation);
                await _storeContext.SaveChangesAsync();
            }
        }

        public async Task DeleteReservations(List<Reservation> reservations)
        {
            _storeContext.Reservations.RemoveRange(reservations);
            await _storeContext.SaveChangesAsync();
        }

        public async Task<List<Reservation>> GetReservationsByPropertyId(string propertyId)
        {
            return await _storeContext.Reservations.Where(r => $"{r.PlatformName}-{r.AccountId}-{r.PlatformPropertyId}" == propertyId).ToListAsync();
        }
    }
}