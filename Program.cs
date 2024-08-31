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
						CLIArgs parsedArgs = new CLIArgs(args);
						SendWoL(parsedArgs.MAC);
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
				static bool CanSendBroadcastPacket(NetworkInterface nif) {
						if (nif.IsReceiveOnly) { return false; }
						if (nif.NetworkInterfaceType == NetworkInterfaceType.Loopback) { return false; }
						if (!nif.Supports(NetworkInterfaceComponent.IPv4)) { return false; }


						return nif.OperationalStatus == OperationalStatus.Up;
				}
				static IEnumerable<IPAddress> GetBroadcastIPs() {
						if (!NetworkInterface.GetIsNetworkAvailable()) { yield break; }
						NetworkInterface[] nifs = NetworkInterface.GetAllNetworkInterfaces();

						NetworkInterface nif;
						IPInterfaceProperties ipInterfaceProperties;
						UnicastIPAddressInformationCollection unitcastAddresses;
						IPAddress addr;
						IPAddress mask;
						IPv4Address iphelper;
						IPAddress result;
						for (int i = 0; i < nifs.Length; i++) {
								nif = nifs[i];
								if (!CanSendBroadcastPacket(nif)) { continue; }
								ipInterfaceProperties = nif.GetIPProperties();
								unitcastAddresses = ipInterfaceProperties.UnicastAddresses;
								foreach (UnicastIPAddressInformation ipinfo in unitcastAddresses) {
										addr = ipinfo.Address;
										if (addr.AddressFamily != AddressFamily.InterNetwork) { continue; } // Also checks that the ip is IPv4
										mask = ipinfo.IPv4Mask;
										iphelper = new(addr, mask);
										result = new IPAddress(iphelper.BroadcastAddress);
										yield return result;
								}
						}
				}
				static void SendWoL(PhysicalAddress mac) {
						ReadOnlySpan<byte> magicPacket = BuildMagicPacket(mac);
#if DEBUG
						Console.WriteLine($"Magic Packet = 0x{Convert.ToHexString(magicPacket)}");
#endif
						using (UdpClient udpClient = new()) {
								foreach (IPAddress ip in GetBroadcastIPs()) {
										udpClient.Connect(ip, 9);
										udpClient.Send(magicPacket);
#if DEBUG
										Console.WriteLine($"Sent WoL packet to: {ip.ToString()}:9");
#endif
								}
						}
				}
		}
}
