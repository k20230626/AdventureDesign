using ADHuffmanCode.Model;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ADHuffmanCode.Message;

public class CompressInfoMessage : ValueChangedMessage<CompressInfo> {
    public CompressInfoMessage(CompressInfo value) : base(value) {
    }
}