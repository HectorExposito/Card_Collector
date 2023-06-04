using SQLite4Unity3d;

public class Card
{
	public int number { get; set; }
	public string collection { get; set; }
	public string rarity { get; set; }
	[PrimaryKey]
	public string image { get; set; }

	public override string ToString()
	{
		return string.Format("[Card: numero={0}, coleccion={1},  rarity={2}, imagen={3}]"
			, number, collection, rarity, image);
	}

	public Card()
    {
    }

	public Card(int number,string collection, string rarity, string image)
	{
		this.number = number;
		this.collection = collection;
		this.rarity = rarity;
		this.image = image;
	}
}
