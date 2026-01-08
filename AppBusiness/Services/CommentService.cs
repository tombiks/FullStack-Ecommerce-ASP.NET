using AppBusiness.DTOs.CommentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppBusiness.Services
{
    public class CommentService
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("data-api");

        public CommentService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<bool> CanUserComment(int userId, int productId)
        {
            try
            {
                var response = await Client.GetAsync($"api/Comment/cancomment/{userId}/{productId}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[CommentService] CanUserComment API failed with status: {response.StatusCode}");
                    return false;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[CommentService] CanUserComment API Response: {responseContent}");

                // Parse the JSON manually since we already read the content
                using var jsonDoc = System.Text.Json.JsonDocument.Parse(responseContent);
                if (jsonDoc.RootElement.TryGetProperty("canComment", out var canCommentElement))
                {
                    var canComment = canCommentElement.GetBoolean();
                    Console.WriteLine($"[CommentService] Final CanComment result: {canComment}");

                    if (jsonDoc.RootElement.TryGetProperty("reason", out var reasonElement))
                    {
                        Console.WriteLine($"[CommentService] Reason: {reasonElement.GetString()}");
                    }

                    if (jsonDoc.RootElement.TryGetProperty("debug", out var debugElement))
                    {
                        Console.WriteLine($"[CommentService] Debug: {debugElement}");
                    }

                    return canComment;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CommentService] CanUserComment exception: {ex.Message}");
                Console.WriteLine($"[CommentService] StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> AddComment(CommentAddDTO commentDto)
        {
            var response = await Client.PostAsJsonAsync("api/Comment/add", commentDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<List<CommentShowDTO>> GetProductComments(int productId)
        {
            var response = await Client.GetAsync($"api/Comment/product/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var comments = await response.Content.ReadFromJsonAsync<List<CommentShowDTO>>();
            return comments ?? new List<CommentShowDTO>();
        }

        public async Task<List<CommentAdminDTO>> GetWaitingComments()
        {
            var response = await Client.GetAsync("api/Comment/admin/waiting");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            var comments = await response.Content.ReadFromJsonAsync<List<CommentAdminDTO>>();
            return comments ?? new List<CommentAdminDTO>();
        }

        public async Task<int> GetWaitingCommentsCount()
        {
            try
            {
                var response = await Client.GetAsync("api/Comment/admin/waiting/count");

                if (!response.IsSuccessStatusCode)
                {
                    return 0;
                }

                var count = await response.Content.ReadFromJsonAsync<int>();
                return count;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> ConfirmComment(int commentId)
        {
            var response = await Client.PutAsync($"api/Comment/admin/confirm/{commentId}", null);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RejectComment(int commentId)
        {
            var response = await Client.DeleteAsync($"api/Comment/admin/reject/{commentId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}
