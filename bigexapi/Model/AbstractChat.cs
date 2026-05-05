using bigexapi.Model.Enum;

namespace bigexapi.Model;

public class AbstractChat
{
    public Guid IchatPublicId {  get; set; }
    public string IchatName { get; set; }
    public string ChannelUniqueName {  get; set; }
    public ChatType ChatType { get; set; }
    public string IchatMainImage {  get; set; }
    public string ichatDescription { get; set; }
}
