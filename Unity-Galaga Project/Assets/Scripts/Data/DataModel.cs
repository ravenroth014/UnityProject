//  DataModel.cs
//  By Atid Puwatnuttasit

using System;
using System.Collections.Generic;

[Serializable]
public class Wave
{
    public List<EnemyType> FirstEnemyList;              // 'EnemyType' list of the first enemy line.
    public List<EnemyType> SecondEnemyList;             // 'EnemyType' list of the second enemy line.
    public Path FirstPath;                              // 'Path' data of the first enemy line.
    public Path SecondPath;                             // 'Path' data of the second enemy line.
}