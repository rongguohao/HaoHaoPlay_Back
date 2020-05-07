﻿namespace Hao.Library
{
    public class AppSettingsInfo
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public JwtOptions JwtOptions { get; set; }

        public RedisPrefixOptions RedisPrefixOptions { get; set; }

        public SnowflakeIdOptions SnowflakeIdOptions { get; set; }

        public RabbitMQ RabbitMQ { get; set; }

        public KeyInfo KeyInfo { get; set; }

        public DataProtectorPurpose DataProtectorPurpose { get; set; }
    }
}
