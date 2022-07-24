namespace QP.Domain.Bag {
  /// <summary>
  /// 由于淘宝不具备商品权限,所以在软件中加入了商品别名的功能,就是一般软件的商品简称功能.当订单产生后,淘宝的订单会根据itemid和skuname生成包裹商品信息名.但是这个名称不够直观,所以这个表可以用于搜索生成的那个码对应的商品简称.这个简称也就是erp平台中的商品名称2021年8月24日16:21:35
  /// </summary>
  interface BagItemsAlias {
    /// <summary>
    /// 流水号 自动产生
    /// </summary>
    Id: number;
    /// <summary>
    /// 商品的所在平台
    /// </summary>
    Platform: string;
    /// <summary>
    /// 商品在他所属平台上的id
    /// </summary>
    PlatformItemId: number;
    /// <summary>
    /// 商品在他所属平台上的sku名称
    /// </summary>
    PlatformSkuName: string;
    /// <summary>
    /// 给这个商品根据itemid+sku名称进行的编码
    /// </summary>
    Alias: string;
    /// <summary>
    /// 在qperp中的商品id
    /// </summary>
    ItemIdAtERP: number;
    /// <summary>
    /// 商品在他所属平台上的标题
    /// </summary>
    PlatformItemTitle: string;
    Memo: string;
  }
}
