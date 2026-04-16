namespace Core.Net
{
    public interface IProtocol
    {
        byte ProtoValue { get; }

        int Opcode { get; }

        byte[] Buffs { get; }
    }
}