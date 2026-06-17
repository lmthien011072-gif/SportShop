using SportShop.Data.Repositories;
using SportShop.Entities.Shop;
using SportShop.Entities;
using SportShop.Models;
using System.Threading.Tasks;
using System;

namespace SportShop.Services
{
    public interface IOrderService
    {
        Task<(bool IsSuccess, string ErrorMessage, int OrderId)> PlaceOrderAsync(int userId, ShoppingCart cart, string shippingAddress, PaymentMethod paymentMethod, bool isCustomized, bool isPartialPayment);
    }

    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool IsSuccess, string ErrorMessage, int OrderId)> PlaceOrderAsync(int userId, ShoppingCart cart, string shippingAddress, PaymentMethod paymentMethod, bool isCustomized, bool isPartialPayment)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var order = new Order(userId, shippingAddress, paymentMethod, isCustomized);

                foreach (var item in cart.Items)
                {
                    var variant = await _unitOfWork.ProductVariant.GetFirstOrDefaultAsync(
                        filter: v => v.Id == item.VariantId,
                        includeProperties: "Product"
                    );

                    if (variant == null)
                    {
                        return (false, $"Sản phẩm {item.ProductName} không còn tồn tại trên hệ thống.", 0);
                    }

                    if (item.Quantity > variant.StockQuantity)
                    {
                        return (false, $"Xin lỗi, sản phẩm '{item.ProductName}' vừa có người khác mua hết hoặc chỉ còn {variant.StockQuantity} cái. Vui lòng quay lại giỏ hàng để điều chỉnh.", 0);
                    }

                    decimal currentRealPrice = variant.Product.Price;

                    order.AddOrderItem(variant.Id, item.Quantity, currentRealPrice);

                    variant.StockQuantity -= item.Quantity;
                    _unitOfWork.ProductVariant.Update(variant);
                }

                if (paymentMethod == PaymentMethod.BankTransfer)
                {
                    if (isPartialPayment)
                        order.SetAdvancePayment(order.TotalAmount * 0.3m); // Cọc 30%
                    else
                        order.SetAdvancePayment(order.TotalAmount); 
                }

                await _unitOfWork.Order.AddAsync(order);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return (true, string.Empty, order.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return (false, "Có lỗi xảy ra trong quá trình xử lý đơn hàng. Vui lòng thử lại sau.", 0);
            }
        }
    }
}