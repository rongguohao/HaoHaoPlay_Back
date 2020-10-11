using FluentValidation;

namespace Hao.AppService
{
    /// <summary>
    /// 更新头像请求
    /// </summary>
    public class UpdateHeadImgRequest
    {
        // public string Base64Str { get; set; } 
        
        /// <summary>
        /// 头像地址
        /// </summary>
        public string HeadImageUrl { get; set; }
    }

    /// <summary>
    /// ��֤
    /// </summary>
    public class UpdateHeadImgValidator : AbstractValidator<UpdateHeadImgRequest>
    {
        public UpdateHeadImgValidator()
        {
            RuleFor(x => x.HeadImageUrl).MustHasValue("头像地址");
        }
    }
}