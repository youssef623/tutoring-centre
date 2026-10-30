namespace TutoringCentre.Domain.Conversations;

public enum ChannelProvider { Whatsapp, Web }
public enum IdentityStatus { Unresolved, Parent, Unknown, Identified }
public enum InboundMessageType { Text, Audio, Image, Other }
public enum InboundMessageStatus { Received, Processed }
