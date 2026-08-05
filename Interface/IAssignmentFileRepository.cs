using Course_Management.Models;

namespace Course_Management.Interface
{
	public interface IAssignmentFileRepository
	{
		/// <summary>
		/// Uploads an assignment file and stores its details in the database.
		/// </summary>
		/// <param name="assignmentFile">Assignment file details.</param>
		/// <returns>Number of affected rows.</returns>
		Task<int> UploadAssignment(AssignmentFile assignmentFile);

		/// <summary>
		/// Retrieves all uploaded assignment files.
		/// </summary>
		/// <returns>List of uploaded assignment files.</returns>
		Task<IEnumerable<AssignmentFile>> GetAssignments();

		/// <summary>
		/// Retrieves an uploaded assignment file by its ID.
		/// </summary>
		/// <param name="id">Assignment file ID.</param>
		/// <returns>Assignment file details.</returns>
		Task<AssignmentFile?> GetAssignmentById(int id);

		/// <summary>
		/// Deletes an uploaded assignment file.
		/// </summary>
		/// <param name="id">Assignment file ID.</param>
		Task DeleteAssignment(int id);
	}
}