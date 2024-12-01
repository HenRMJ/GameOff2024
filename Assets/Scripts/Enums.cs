using System;

public enum BuildingTypes
{
    Kitchen,
    School,
    Avatar,
    Foundry
}

public enum CultistTypes
{
    Follower,
    Farmer,
    Builder
}

[Serializable]
public enum CultResources
{
    None,
    Wood,
    Rock,
    Water,
    Meat,
    Vegetables,
}