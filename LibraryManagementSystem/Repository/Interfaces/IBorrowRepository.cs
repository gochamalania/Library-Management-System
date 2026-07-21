using Core.Models;

namespace Repository.Interfaces;

public interface IBorrowRepository
{
    List<BorrowRecord> GetAllBorrows();
    BorrowRecord GetById(string id);
    void AddBorrow(BorrowRecord record);
    void UpdateBorrow(BorrowRecord record);
}