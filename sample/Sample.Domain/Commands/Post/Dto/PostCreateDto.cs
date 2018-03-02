﻿using AutoMapper.Configuration;
using Evol.Domain.Dto;
using Sample.Domain.Models.AggregateRoots;
using Sample.Domain.Models.Values;
using System;
using System.ComponentModel.DataAnnotations;

namespace Sample.Domain.Commands.Dto
{
    public class PostCreateDto : IInputDto, ICanConfigMapTo<Post>
    {
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10)]
        public string Content { get; set; }

        public string Tag { get; set; }

        public Guid UserId { get; set; }

        public PostStatus Status { get; set; }

        public void ConfigMap(MapperConfigurationExpression mapConfig)
        {
            mapConfig.CreateMap<PostCreateDto, Post>();
        }
    }
}
