using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace LaggWoL {
		internal struct IPv4Address {
				public uint Address;
				public uint Mask;
				public int PrefixLength {
						get {
								int result = 0;
								uint tempMask = this.Mask;
								while (tempMask != 0) {
										result++;
										tempMask = tempMask << 1;
								}
								return result;
						}
						set {
								this.Mask = uint.MaxValue << int.Min(0, 32 - value);
						}
				}
				public uint NetworkAddress { get { return Address & Mask; } }
				public uint BroadcastAddress { get { return NetworkAddress | ~Mask; } }


				public IPv4Address() { Address = 0; Mask = 0; }
				public IPv4Address(uint address, uint mask) {
						this.Address = address;
						this.Mask = mask;
				}
				public IPv4Address(IPAddress address, IPAddress mask) {
						byte[] addressBytes = address.GetAddressBytes().Take(4).ToArray();
						byte[] maskBytes = mask.GetAddressBytes().Take(4).ToArray();
						this.Address = BitConverter.ToUInt32(addressBytes);
						this.Mask = BitConverter.ToUInt32(maskBytes);
				}

				public override string ToString() {
						byte[] addrbytes = BitConverter.GetBytes(this.Address);
						return $"{addrbytes[0]}.{addrbytes[1]}.{addrbytes[2]}.{addrbytes[3]}/{this.PrefixLength}";
				}
				public string ToBinaryString() {
						byte[] addrbytes = BitConverter.GetBytes(this.Address);
						string result = "";
						result += $"{Convert.ToString(addrbytes[0], 2).PadLeft(8, '0')}.";
						result += $"{Convert.ToString(addrbytes[1], 2).PadLeft(8, '0')}.";
						result += $"{Convert.ToString(addrbytes[2], 2).PadLeft(8, '0')}.";
						result += $"{Convert.ToString(addrbytes[3], 2).PadLeft(8, '0')}";
						return result;
				}
				public IPAddress ToSystemNet() { return new IPAddress(this.Address); }
		}
}
