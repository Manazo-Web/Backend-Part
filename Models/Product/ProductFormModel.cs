namespace Manazo.Models.Product;
using System.Collections.Generic;

public class ProductFormModel
{
    public String ProductId { get; set; } = null!;
    public String SalerId { get; set; } = null!;
    public String ProductName { get; set; } = null!;
    public String? Description { get; set; }
    public String Categories { get; set; } = null!;
    public decimal Price { get; set; }
    public String Photo { get; set; } = null!;
    public List<Review>? Reviews { get; set; }

    private double AverageRating;
    public int StockQuantity { get; set; }

    public double? GetAverageRating(){

        if (Reviews is null) return null;
        AverageRating = 0;
        foreach (Review review in Reviews)
        {
            AverageRating += review.Rating;
        }
        AverageRating /= Reviews.Count;
        return AverageRating;
    }

    bool IsDeleted;

    public void DeleteMe()
    {
        IsDeleted = true;
    }
}
public class Review
{
    public double Rating { get; set; }
    public String? Comment { get; set; }
}

