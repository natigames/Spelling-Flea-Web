using System.Collections.Generic;
using System;

[Serializable]
public class user
{
    public bool success;
    public string error;
    public int userid;
    public string email;
}


[Serializable]
public class info
{
    public string level;
    public string packname;
    public int leftwords;
    public int usedwords;
}

[Serializable]
public class tips
{
    public string word;
    public string definition;
    public string examples;
    public string synonyms;
    public string derivation;
}

[Serializable]
public class packs
{
    public int packid;
    public string packname;
    public int userid;
}

[Serializable]
public class Packslist
{
    public List<packs> Packs = new List<packs>();
}


[Serializable]
public class leaders
{
    public string name1;
    public string name2;
    public string name3;
    public string name4;
    public string name5;
    public string nameyou;
    public string total1;
    public string total2;
    public string total3;
    public string total4;
    public string total5;
    public string totalyou;
    public string score1;
    public string score2;
    public string score3;
    public string score4;
    public string score5;
    public string scoreyou;
    public string level;
    public string packname;
}