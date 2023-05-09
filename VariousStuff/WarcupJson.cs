
using System;

public class WarcupData
{
    public string id { get; set; }
    public string header { get; set; }
    public string headerEn { get; set; }
    public string text { get; set; }
    public string textEn { get; set; }
    public string youtube { get; set; }
    public DateTime datetimezone { get; set; }
    public string authorId { get; set; }
    public string multicupId { get; set; }
    public string image { get; set; }
    public string cupId { get; set; }
    public string authorName { get; set; }
    public string type { get; set; }
    public string tableJson { get; set; }
    public string currentRound { get; set; }
    public DateTime startDateTime { get; set; }
    public string twitch1 { get; set; }
    public string twitch2 { get; set; }
    public Cup cup { get; set; }
    public string levelshot { get; set; }
    public Results cpmResults { get; set; }
    public Results vq3Results { get; set; }
    public Comment[] comments { get; set; }
    public bool preposted { get; set; }
}

public class Cup
{
    public string id { get; set; }
    public string fullName { get; set; }
    public string shortName { get; set; }
    public string youtube { get; set; }
    public string twitch { get; set; }
    public string currentRound { get; set; }
    public DateTime startDateTime { get; set; }
    public DateTime endDateTime { get; set; }
    public string server1 { get; set; }
    public string server2 { get; set; }
    public string map1 { get; set; }
    public string map2 { get; set; }
    public string map3 { get; set; }
    public string map4 { get; set; }
    public string map5 { get; set; }
    public string physics { get; set; }
    public string type { get; set; }
    public string mapWeapons { get; set; }
    public string mapAuthor { get; set; }
    public string mapPk3 { get; set; }
    public string mapSize { get; set; }
    public string timer { get; set; }
    public string ratingCalculated { get; set; }
    public string useTwoServers { get; set; }
    public string demosValidated { get; set; }
    public string multicupId { get; set; }
    public string archiveLink { get; set; }
    public string bonusRating { get; set; }
    public string system { get; set; }
    public string customMap { get; set; }
    public string customNews { get; set; }
    public string multicup { get; set; }
}

public class Results
{
    public Entry[] valid { get; set; }
    public Entry[] invalid { get; set; }
}

public class Entry
{
    public string time { get; set; }
    public string nick { get; set; }
    public string demopath { get; set; }
    public string country { get; set; }
    public string rating { get; set; }
    public string playerId { get; set; }
   // public object change { get; set; }
    public string col { get; set; }
    public string row { get; set; }
  //  public object bonus { get; set; }
    public string impressive { get; set; }
}

/*
public class Vq3results
{
    public Entry[] valid { get; set; }
    public Entry[] invalid { get; set; }
}
public class Valid1
{
    public string time { get; set; }
    public string nick { get; set; }
    public string demopath { get; set; }
    public string country { get; set; }
    public string rating { get; set; }
    public string playerId { get; set; }
    public object change { get; set; }
    public string col { get; set; }
    public string row { get; set; }
    public object bonus { get; set; }
    public string impressive { get; set; }
}*/

public class Comment
{
    public string id { get; set; }
    public string newsId { get; set; }
    public string userId { get; set; }
    public string comment { get; set; }
    public DateTime datetimezone { get; set; }
    public string reason { get; set; }
    public string username { get; set; }
    public string playerId { get; set; }
}
