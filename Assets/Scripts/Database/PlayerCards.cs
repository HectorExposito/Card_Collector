using SQLite4Unity3d;

public class PlayerCards
{
	public int number { get; set; }
	public string collection { get; set; }
	public string rarity { get; set; }
	[PrimaryKey]
	public string image { get; set; }

	public int quantity { get; set; }

	public override string ToString()
	{
		return string.Format("[Card: numero={0}, coleccion={1},  rarity={2}, imagen={3}]"
			, number, collection, rarity, image);
	}

	public PlayerCards()
	{
	}

	public PlayerCards(int number, string collection, string rarity, string image, int quantity)
	{
		this.number = number;
		this.collection = collection;
		this.rarity = rarity;
		this.image = image;
		this.quantity = quantity;
	}

    public PlayerCards(Card card, int quantity)
    {
		this.number = card.number;
		this.collection = card.collection;
		this.rarity = card.rarity;
		this.image = card.image;
		this.quantity = quantity;
    }
}
