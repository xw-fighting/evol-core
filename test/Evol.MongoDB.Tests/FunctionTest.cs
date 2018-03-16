﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Evol.Common;
using Evol.MongoDB.Repository;
using Evol.MongoDB.Test.Entities;
using Evol.MongoDB.Test.Repository;
using Evol.Util;
using Xunit;
using MongoDB.Driver;
using System.ComponentModel;

namespace Evol.MongoDB.Test
{
    public class FunctionTest
    {
        private static UserRepository _userRepository;

        public FunctionTest()
        {
            _userRepository = new UserRepository(new DefaultMongoDbContextProvider());
            InitClear();
            InitData();
        }

        public static void InitClear()
        {
            var list = _userRepository.SelectAsync(null, e => e.Id).GetAwaiter().GetResult().ToList();
            var ids = list.Skip(5).Select(e => e.Id).ToList();
            var deleted = _userRepository.DeleteBatchAsync(ids).GetAwaiter().GetResult();
        }

        private static void InitData()
        {
            for (int i = 0; i < 50; i++)
            {
                var gender = FakeUtil.CreateGender();
                var item = new User()
                {
                    Username = FakeUtil.CreateUsername(5, 10),
                    Password = FakeUtil.CreatePassword(),
                    Email = FakeUtil.CreateEmail(),
                    Name = FakeUtil.CreatePersonName(gender),
                    Gender = gender,
                    Age = FakeUtil.RandomInt(18, 50),
                    CreateTime = DateTime.Now
                };

                _userRepository.AddAsync(item).GetAwaiter().GetResult();
                Assert.True(true);
            }
        }



        private User FakeItem()
        {
            var paged = _userRepository.PagedSelectAsync(null, 1, 1).GetAwaiter().GetResult();
            var first = paged.Items.FirstOrDefault();
            return first;
        }

        private List<User> FakeItems(int num)
        {
            var paged = _userRepository.PagedSelectAsync(null, 1, num).GetAwaiter().GetResult();
            var list = paged.Items.ToList();
            return list;
        }

        [Fact,Description("Insert")]
        public void AddOneTest()
        {
            var gender = FakeUtil.CreateGender();
            var item = new User()
            {
                Username = FakeUtil.CreateUsername(5, 10),
                Password = FakeUtil.CreatePassword(),
                Email = FakeUtil.CreateEmail(),
                Name = FakeUtil.CreatePersonName(gender),
                Gender = gender,
                Age = FakeUtil.RandomInt(18, 50),
                CreateTime = DateTime.Now
            };

            _userRepository.AddAsync(item).GetAwaiter().GetResult();
            Assert.True(true);
        }

        [Fact, Description("Insert Batch")]
        public void AddBatchTest()
        {
            var list = new List<User>();
            for (int i = 0; i < 10; i++)
            {
                var gender = FakeUtil.CreateGender();
                var item = new User()
                {
                    Username = FakeUtil.CreateUsername(5, 10),
                    Password = FakeUtil.CreatePassword(),
                    Email = FakeUtil.CreateEmail(),
                    Name = FakeUtil.CreatePersonName(gender),
                    Gender = gender,
                    Age = FakeUtil.RandomInt(18, 50),
                    CreateTime = DateTime.Now
                };
                list.Add(item);
            }

            _userRepository.AddBatchAsync(list).GetAwaiter().GetResult();
            Assert.True(true);
        }





        [Fact, Description("Update Entity")]
        public void UpdateOneEntityTest()
        {
            var fakeUser = FakeItem();
            fakeUser.Gender = FakeUtil.CreateGender();
            fakeUser.Password = FakeUtil.CreatePassword();
            fakeUser.Email = FakeUtil.CreateEmail();
            fakeUser.Age = FakeUtil.RandomInt(18, 50);
            fakeUser.UpdateTime = DateTime.Now;

            var updated = _userRepository.UpdateAsync(fakeUser).GetAwaiter().GetResult();
            Assert.True(updated);
        }

        [Fact, Description("Update-Definition")]
        public void UpdateOneDefinitionTest()
        {
            var fakeUser = FakeItem();
            Trace.WriteLine($"Update Id:{fakeUser.Id}");
            var updateDef = Builders<User>.Update.Set(e => e.Gender, GenderType.Female)
                .Set(e => e.Email, "0000@8010.net")
                .Set(e => e.Name, fakeUser.Name + "0001")
                .Set(e => e.UpdateTime, DateTime.Now);
            var updated = _userRepository.UpdateAsync(fakeUser.Id, updateDef).GetAwaiter().GetResult();
            Assert.True(updated);
        }


        [Fact,Description("Multi-FindOne")]
        public void FindOneTest()
        {
            var fakeUser = FakeItem();

            var user = _userRepository.FindAsync(fakeUser.Id).GetAwaiter().GetResult();
            Assert.NotNull(user);

            var user1 = _userRepository.FindOneAsync(e => e.Email == fakeUser.Email).GetAwaiter().GetResult();
            Assert.NotNull(user1);

            Expression<Func<User, bool>> express = u => u.Email == fakeUser.Email;
            FilterDefinition<User> filter = express;
            var user2 = _userRepository.FindOneAsync(filter).GetAwaiter().GetResult();
            Assert.NotNull(user2);
        }

        [Fact,Description("Multi-Queryable")]
        public void QueryableTest()
        {
            var all = _userRepository.AsQueryable().ToList();
            Trace.WriteLine($"all user count:{all.Count}");
            Assert.True(all.Count > 0);

            var all1 = _userRepository.Queryable(e => e.Gender == GenderType.Female, e => e.Id).ToList();
            Trace.WriteLine($"all1 user count:{all1.Count}");
            Assert.True(all1.Count > 0);

            Expression<Func<User, bool>> express = u => u.Email.Contains("@qq.com");
            FilterDefinition<User> filter = express;
            var all2 = _userRepository.FluentQueryable(filter, "CreateTime").ToList();
            Assert.True(all2.Count > 0);
            Trace.WriteLine($"all2 user count:{all1.Count}");

            foreach (var item in all)
            {
                Trace.WriteLine($"Id:{item.Id}, Username:{item.Username}, Password:{item.Password}, Email:{item.Email}, Name:{item.Name}, Gender:{item.Gender}, Age:{item.Age}, CreateTime:{item.CreateTime}");
            }
            Assert.NotNull(all);
        }

        [Fact, Description("Multi-Select")]
        public void SelectTest()
        {
            var list = _userRepository.SelectAsync(e => e.Gender == GenderType.Female, e => e.CreateTime).GetAwaiter().GetResult();
            Trace.WriteLine($"all user count:{list.Count}");
            Assert.True(list.Count > 0);

            Expression<Func<User, bool>> express = u => u.Email.Contains(".com");
            FilterDefinition<User> filter = express;
            var list1 = _userRepository.SelectAsync(filter, "CreateTime").GetAwaiter().GetResult();
            Assert.True(list1.Count > 0);
            Trace.WriteLine($"list1 count:{list1.Count}");

        }

        [Fact, Description("Multi-PageSelect")]
        public void PagedTest()
        {
            var paged = _userRepository.PagedSelectAsync(e => e.Gender == GenderType.Male, 2, 10).GetAwaiter().GetResult();
            Trace.WriteLine($"page total:{paged.PageTotal}, record total:{paged.RecordTotal}, page index:{paged.PageIndex}, page size:{paged.PageSize}");
            Assert.True(paged.Items.Any());

            Expression<Func<User, bool>> express = u => u.Email.Contains(".com");
            FilterDefinition<User> filter = express;
            var paged1 = _userRepository.PagedSelectAsync(e => e.Gender == GenderType.Female, 2, 10).GetAwaiter().GetResult();
            Trace.WriteLine($"page total:{paged1.PageTotal}, record total:{paged1.RecordTotal}, page index:{paged1.PageIndex}, page size:{paged1.PageSize}");
            Assert.True(paged1.Items.Any());
            Trace.WriteLine($"paged1 count:{paged1.Items.Count()}");

        }


        [Fact, Description("Delete")]
        public void DeleteOneTest()
        {
            var fakeUser = FakeItem();
            var deleted = _userRepository.DeleteAsync(fakeUser.Id).GetAwaiter().GetResult();
            Assert.True(deleted);
        }

        [Fact, Description("Delete Batch")]
        public void DeleteBatchTest()
        {
            var fakeUsers = FakeItems(10);
            var ids = fakeUsers.Select(e => e.Id).ToList();
            var deleted = _userRepository.DeleteBatchAsync(ids).GetAwaiter().GetResult();
            Assert.True(deleted);
        }

        [Fact, Description("Delete By PredicateTest")]
        public void DeleteByPredicateTest()
        {
            var deleted = _userRepository.DeleteByAsync(e => e.Age > 40).GetAwaiter().GetResult();
            Assert.True(deleted);
        }

        [Fact, Description("Delete By Filter")]
        public void DeleteByFilterTest()
        {
            Expression<Func<User, bool>> express = u => u.Age < 30;
            FilterDefinition<User> filter = express;
            var deleted = _userRepository.DeleteByAsync(filter).GetAwaiter().GetResult();
            Assert.True(deleted);
        }
    }
}
