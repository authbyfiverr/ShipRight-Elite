using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LockedBitmapUtil;

namespace ShipRight
{
	public class Piece : IEqualityComparer<Piece>
	{
		public string Name { get; set; }
		public int[][] Shape { get; set; }
		public Dictionary<Tile, int> Requirements { get; set; }
		public int Size { get; set; }
		public LockedBitmap ImageRef { get; set; }
		public int Index { get; set; }
		public Point IndexPoint { get; set; }
		public Point OriginPoint { get; set; }

		//Solving Properties
		public bool Used { get; set; } = false;
		public bool PartialUsed { get; set; } = false;
		public int[][] BoardPos { get; set; }
		public bool IsBonus { get; set; } = false;


		public Piece(string name, int[][] shape, int size, LockedBitmap imageRef, Point originPoint)
		{
			Name = name;
			Shape = shape;
			Requirements = GenerateRequirements(shape);
			Size = size;
			ImageRef = imageRef;
			BoardPos = new int[5][]
			{
				new int[5],
				new int[5],
				new int[5],
				new int[5],
				new int[5]
			};
			OriginPoint = originPoint;
		}

		public Piece Clone()
		{
			return new Piece(Name, Shape.Select(row => row.ToArray()).ToArray(), Size, ImageRef, OriginPoint)
			{
				Index = this.Index,
				IndexPoint = this.IndexPoint,
				Used = this.Used,
				Requirements = new Dictionary<Tile, int>(this.Requirements),
				BoardPos = this.BoardPos.DeepClone(),
				IsBonus = this.IsBonus
			};
		}
		public bool Equals(Piece x, Piece y)
		{
			return x.Name == y.Name;
		}

		public int GetHashCode(Piece obj)
		{
			return obj.Name.GetHashCode();
		}

		private Dictionary<Tile, int> GenerateRequirements(int[][] shape)
		{
			Dictionary<Tile, int> requirements = new Dictionary<Tile, int>();

			foreach (var row in Shape)
			{
				foreach (var tile in row)
				{
					if (tile != 0) // Ignore empty spaces
					{
						Tile tileType = (Tile)tile;
						if (requirements.ContainsKey(tileType))
						{
							requirements[tileType]++;
						}
						else
						{
							requirements[tileType] = 1;
						}
					}
				}
			}

			return requirements;

		}

	}


	enum Pieces
	{
		Batten,
		Bobstay,
		Bollard,
		Cannon,
		Cleat,
		Cringle,
		Flag,
		Gaff,
		Halyard,
		Hatch,
		Jib,
		Knee,
		Pump,
		Rigging,
		Shackle,
		Thimble,
		Ballast,
		Barrel,
		Block,
		Boltrope,
		Boom,
		Mast,
		Rudder,
		Sail,
		Shroud,
		Yard,
		Anchor,
		Berth,
		Bowspirit,
		Capstan,
		Gangway,
		Helm,
		Lateen,
		Mooring,
		Nest,
		Shot

	}


}
