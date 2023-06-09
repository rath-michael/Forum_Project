﻿using Forum_API_Provider.Models.ForumModels;
using Forum_API_Provider.Models.ForumModels.Posts;
using Forum_API_Provider.Models.ForumModels.Rooms;
using Forum_API_Provider.Models.ForumModels.Users;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Forum_API_Provider.Services.ForumService
{
    public class ForumRepository : IForumRepository
    {
        private ForumDbContext context;
        public ForumRepository(ForumDbContext context)
        {
            this.context = context;
        }

        #region Room
        public async Task<List<Room>> GetAllRooms()
        {
            var rooms = await context.Rooms.ToListAsync();
            foreach (var room in rooms)
            {
                room.PostCount = await context.Posts
                    .Where(p => p.RoomId == room.RoomId)
                    .CountAsync();
            }
            return rooms;
        }
        public async Task<Room> GetRoom(int roomId)
        {
            return await context.Rooms
                .Where(r => r.RoomId == roomId)
                .SingleOrDefaultAsync();
        }
        public async Task<bool> AddRoom(AddRoomRequest request)
        {
            return true;
        }
        public async Task<bool> UpdateRoom(Room room)
        {
            return true;
        }
        public async Task<bool> DeleteRoom(int roomId)
        {
            return true;
        }
        #endregion

        #region Post
        public async Task<Post> GetPost(int postId)
        {
            return await context.Posts
                .Where(p => p.PostId == postId)
                .SingleOrDefaultAsync();
        }
        public async Task<List<Post>> GetAllPosts()
        {
            return await context.Posts.ToListAsync();
        }
        public async Task<PostsByRoomResponse> GetPostsByRoom(Room room)
        {
            var posts = await context.Posts.Where(p => p.RoomId == room.RoomId).ToListAsync();
            var response = new PostsByRoomResponse
            {
                Success = true,
                Message = "",
                Room = room,
                Posts = posts
            };
            return response;
        }
        public async Task<PostsByUserResponse> GetPostsByUser(User user)
        {
            var posts = await context.Posts.Where(p => p.UserId == user.UserId).ToListAsync();
            var response = new PostsByUserResponse
            {
                Success = true,
                Message = "",
                User = user,
                Posts = posts
            };
            return response;
        }
        public async Task<AddPostResponse> AddPost(Post post)
        {
            await context.Posts.AddAsync(post);
            var response = await context.SaveChangesAsync();

            if (response >= 0)
            {
                return new AddPostResponse
                {
                    Success = true,
                    Message = "Post successfully added",
                    Post = post
                };
            }

            return new AddPostResponse 
            { 
                Success = false,
                Message = "Post add failure"
            };
        }
        public async Task<UpdatePostResponse> UpdatePost(Post updatedPost, Post originalPost)
        {
            updatedPost.Title = originalPost.Title;
            updatedPost.Message = originalPost.Message;
            var response = await context.SaveChangesAsync();

            if (response >= 0)
            {
                return new UpdatePostResponse
                {
                    Success = true,
                    Message = "Post successfully updated",
                    Post = updatedPost
                };
            }

            return new UpdatePostResponse
            {
                Success = false,
                Message = "Unable to update post"
            };
        }
        public async Task<DeletePostResponse> DeletePost(Post post)
        {
            context.Posts.Remove(post);
            var response = await context.SaveChangesAsync();

            if (response >= 0)
            {
                return new DeletePostResponse
                {
                    Success = true,
                    Message = "Post successfully deleted"
                };
            }

            return new DeletePostResponse
            {
                Success = false,
                Message = "Unable to delete post"
            };
        }
        #endregion

        #region User
        public async Task<User> GetUser(int userId)
        {
            return await context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
        }
        #endregion
    }
}
