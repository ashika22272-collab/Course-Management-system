
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.Repositories;

namespace CourseManagementAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentRepository _paymentRepository;

		public PaymentController(
			IPaymentRepository paymentRepository)
		{
			_paymentRepository = paymentRepository;
		}


		// =====================================================
		// GET PAYMENT HISTORY BY STUDENT ID
		// =====================================================

		[HttpGet("student/{userId}")]
		public async Task<IActionResult> GetPaymentsByStudent(
			int userId)
		{
			try
			{
				if (userId <= 0)
				{
					return BadRequest(new
					{
						message = "Invalid User ID."
					});
				}

				var payments =
					await _paymentRepository
						.GetPaymentsByStudentId(userId);

				return Ok(payments);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message =
						"An error occurred while retrieving payment history.",

					error = ex.Message
				});
			}
		}


		// =====================================================
		// GET PAYMENTS FOR INSTRUCTOR
		// =====================================================

		[HttpGet("instructor/{instructorId}")]
		public async Task<IActionResult> GetPaymentsByInstructor(
			int instructorId)
		{
			try
			{
				if (instructorId <= 0)
				{
					return BadRequest(new
					{
						message = "Invalid Instructor ID."
					});
				}

				var payments =
					await _paymentRepository
						.GetPaymentsByInstructorId(instructorId);

				return Ok(payments);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message =
						"An error occurred while retrieving instructor payments.",

					error = ex.Message
				});
			}
		}
	}
}

