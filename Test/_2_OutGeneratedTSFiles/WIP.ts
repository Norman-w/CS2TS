/// <summary>
/// 检查二维码是否有效的响应
/// </summary>
export class CheckQrCodeResponse implements BaseResponse {
    OpenId?: string;
    // public string? NickName { get; set; }
    //
    // public string? HeadImgUrl { get; set; }
    //
    // public string? UnionId { get; set; }
    SessionKey?: string;
    GameStartGuid?: string;
}