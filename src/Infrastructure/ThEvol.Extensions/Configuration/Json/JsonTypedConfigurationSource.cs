﻿using System;

namespace Evol.Extensions.Configuration.Json
{
    public class JsonTypedConfigurationSource : FileTypedConfigurationSource
    {
        public JsonTypedConfigurationSource(Type strongType) : base(strongType)
        {
        }

        public override ITypedConfigurationProvider Build(ITypedConfigurationBuilder builder)
        {
            base.EnsureDefaults(builder);
            return new JsonTypedConfigurationProvider(this);
        }

    }
}
