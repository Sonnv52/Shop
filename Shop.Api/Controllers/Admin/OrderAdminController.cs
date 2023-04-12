﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Shop.Api.Models.Order;
using Shop.Api.Abtracst;
using Shop.Api.Data;
using System.Drawing.Printing;
using Shop.Api.Models.Page;
using X.PagedList;

namespace Shop.Api.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderAdminController : ControllerBase
    {
        private readonly IOrderServices _orderServices;
        private readonly NewDBContext _dbContext;
        public OrderAdminController(NewDBContext dbContext, IOrderServices orderServices) { 
            _orderServices = orderServices;
            _dbContext = dbContext;
        }
        [HttpPatch]
        [Route("/SetBill/Status")]
        // [Authorize(AuthenticationSchemes = "Bearer"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetBillAsync(IList<SetBillRequest> setBills)
        {
            var result = await _orderServices.SetBillAsync(setBills);
            return Ok(result);
        }
        [HttpGet]
        [Route("/GetAllBill")]
        // [Authorize(AuthenticationSchemes = "Bearer"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBillAsync(int page, int pageSize)
        {
            var bill = _dbContext.Bills.ToList();
            var retu = bill.Select(b => new BillAdminDTO
            {
                Adress= b.Adress,
                Id= b.Id,
                OderDate = b.OderDate
            }).ToList().GetPage(page, pageSize);
            return Ok(retu);
        }

    }
}
