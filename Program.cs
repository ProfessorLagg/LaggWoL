using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace LaggWoL {
		internal class Program {
				private sealed class CLIArgs {
						public readonly PhysicalAddress MAC;

						public CLIArgs(IList<string> args) {
								if (!PhysicalAddress.TryParse(args[0], out PhysicalAddress? mac)) {
										throw new ArgumentException($"Could not parse MAC Address '{args[0]}'");
								}
								this.MAC = mac!;
						}
				}
				static void Main(string[] args) {
						try {
								CLIArgs parsedArgs = new CLIArgs(args);
								SendWoL(parsedArgs.MAC);
						}
						catch (Exception ex) {
								PrintHelp(ex.Message);
						}
				}

				static void PrintHelp(string errorMsg = "") {
						// TODO
				}

				private static readonly byte[] MagicPacketPrepend = [0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];
				private static ReadOnlySpan<byte> BuildMagicPacket(PhysicalAddress mac) {
						ReadOnlySpan<byte> macBytes = mac.GetAddressBytes();

						Span<byte> magicPacket = new byte[102];
						MagicPacketPrepend.CopyTo(magicPacket);
						macBytes.CopyTo(magicPacket.Slice(6));
						macBytes.CopyTo(magicPacket.Slice(12));
						macBytes.CopyTo(magicPacket.Slice(18));
						macBytes.CopyTo(magicPacket.Slice(24));
						macBytes.CopyTo(magicPacket.Slice(30));
						macBytes.CopyTo(magicPacket.Slice(36));
						macBytes.CopyTo(magicPacket.Slice(42));
						macBytes.CopyTo(magicPacket.Slice(48));
						macBytes.CopyTo(magicPacket.Slice(54));
						macBytes.CopyTo(magicPacket.Slice(60));
						macBytes.CopyTo(magicPacket.Slice(66));
						macBytes.CopyTo(magicPacket.Slice(72));
						macBytes.CopyTo(magicPacket.Slice(78));
						macBytes.CopyTo(magicPacket.Slice(84));
						macBytes.CopyTo(magicPacket.Slice(90));
						macBytes.CopyTo(magicPacket.Slice(96));

						return magicPacket;
				}
				static void SendWoL(PhysicalAddress mac) {
						ReadOnlySpan<byte> magicPacket = BuildMagicPacket(mac);
						UdpClient udpClient = new UdpClient();
						udpClient.Connect(IPAddress.Broadcast, 9);
						udpClient.Send(magicPacket);
        }
		}
}
