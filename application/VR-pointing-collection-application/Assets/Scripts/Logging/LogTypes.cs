using UnityEngine;
using CsvHelper.Configuration;
using Newtonsoft.Json;

public enum Handedness { Right, Left };
public enum Gender { Female, Male, Other };

/* TYPES USED FOR LOGGING TO CSV */

public class LogParticipant {
    public int id { get; set; }
    public string handedness { get; set; }
    public string gender { get; set; }
    public float age { get; set; }
    public float forearmLength { get; set; }
    public float forearmMarkerDist { get; set; }
    public float fingerLength { get; set; }
    public float upperarmLength { get; set; }
    public float upperarmMarkerDist { get; set; }
    public float height { get; set; }
    // public float armLength { get; set; }
    public float rightShoulderMarkerDistX { get; set; }
    public float rightShoulderMarkerDistY { get; set; }

    public LogParticipant() {}

    public LogParticipant fromParticipant(Participant participant) {
        id = participant.id;
        handedness = participant.handedness.ToString();
        gender = participant.gender.ToString();
        age = participant.age;
        forearmLength = participant.forearmLength;
        forearmMarkerDist = participant.forearmMarkerDist;
        fingerLength = participant.fingerLength;
        upperarmLength = participant.upperarmLength;
        upperarmMarkerDist = participant.upperarmMarkerDist;
        height = participant.height;
        // armLength = participant.armLength;
        rightShoulderMarkerDistX = participant.rightShoulderMarkerDistX;
        rightShoulderMarkerDistY = participant.rightShoulderMarkerDistY;
        return this;
    }
}

public sealed class LogParticipantMap : ClassMap<LogParticipant> {
    public LogParticipantMap() { AutoMap(); }
}

public class Position {
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public Position() {}
    public Position(float x, float y, float z) {
        this.x = x; this.y = y; this.z = z;
    }

    public static Position ToPosition(Vector3 v) {
        // map to real world scale right away
        return new Position(v.x/2f, v.y/2f, v.z/2f);
    }
    public static Position ToPosition(Quaternion v) {
        return new Position(v.x, v.y, v.z);
    }
}

public class Collection {
    public int id { get; set; }
    public Position activeTargetPosition { get; set; }
    public Collection() {}
}

public sealed class CollectionMap : ClassMap<Collection> {
    public CollectionMap() {
        Map( m => m.id ).Name( "id" );
        References<ActiveTargetMap>( m => m.activeTargetPosition );
    }
}


public class ActiveTargetMap : ClassMap<Position> {
    public ActiveTargetMap() {
        Map( m => m.x ).Name( "trueTarget.X" );
        Map( m => m.y ).Name( "trueTarget.Y" );
        Map( m => m.z ).Name( "trueTarget.Z" );
    }
}

public class Snapshot {
    // the frame in which the snapshot was created
    public float timestamp { get; set; }
    public Position indexFinger { get; set; }
    public Position indexFingerO { get; set; }
    public Position hand { get; set; }
    public Position handO { get; set; }
    public Position lowerArm { get; set; }
    public Position lowerArmO { get; set; }
    public Position upperArm { get; set; }
    public Position upperArmO { get; set; }
    public Position leftShoulder { get; set; }
    public Position leftShoulderO { get; set; }
    public Position rightShoulder { get; set; }
    public Position rightShoulderO { get; set; }
    public Position hmd { get; set; }
    public Position hmdO { get; set; }

    public override string ToString() { return JsonConvert.SerializeObject(this); }
}


public sealed class SnapshotMap : ClassMap<Snapshot> {
    public SnapshotMap() {
        Map( m => m.timestamp ).Name( "time" );
        References<IndexFingerMap>( m => m.indexFinger );
        References<IndexFingerOMap>( m => m.indexFingerO );
        References<HandMap>( m => m.hand );
        References<HandOMap>( m => m.handO );
        References<LowerArmMap>( m => m.lowerArm );
        References<LowerArmOMap>( m => m.lowerArmO );
        References<UpperArmMap>( m => m.upperArm );
        References<UpperArmOMap>( m => m.upperArmO );
        References<LeftShoulderMap>( m => m.leftShoulder );
        References<LeftShoulderOMap>( m => m.leftShoulderO );
        References<RightShoulderMap>( m => m.rightShoulder );
        References<RightShoulderOMap>( m => m.rightShoulderO );
        References<HmdMap>( m => m.hmd );
        References<HmdOMap>( m => m.hmdO );
    }
}

public class IndexFingerMap : ClassMap<Position> {
    public IndexFingerMap() {
        Map( m => m.x ).Name( "indexfinger.X" );
        Map( m => m.y ).Name( "indexfinger.Y" );
        Map( m => m.z ).Name( "indexfinger.Z" );
    }
}

public class IndexFingerOMap : ClassMap<Position> {
    public IndexFingerOMap() {
        Map( m => m.x ).Name( "indexfingerO.X" );
        Map( m => m.y ).Name( "indexfingerO.Y" );
        Map( m => m.z ).Name( "indexfingerO.Z" );
    }
}


public class HandMap : ClassMap<Position> {
    public HandMap() {
        Map( m => m.x ).Name( "hand.X" );
        Map( m => m.y ).Name( "hand.Y" );
        Map( m => m.z ).Name( "hand.Z" );
    }
}

public class HandOMap : ClassMap<Position> {
    public HandOMap() {
        Map( m => m.x ).Name( "handO.X" );
        Map( m => m.y ).Name( "handO.Y" );
        Map( m => m.z ).Name( "handO.Z" );
    }
}

public class LowerArmMap : ClassMap<Position> {
    public LowerArmMap() {
        Map( m => m.x ).Name( "forearm.X" );
        Map( m => m.y ).Name( "forearm.Y" );
        Map( m => m.z ).Name( "forearm.Z" );
    }
}

public class LowerArmOMap : ClassMap<Position> {
    public LowerArmOMap() {
        Map( m => m.x ).Name( "forearmO.X" );
        Map( m => m.y ).Name( "forearmO.Y" );
        Map( m => m.z ).Name( "forearmO.Z" );
    }
}

public class UpperArmMap : ClassMap<Position> {
    public UpperArmMap() {
        Map( m => m.x ).Name( "upperarm.X" );
        Map( m => m.y ).Name( "upperarm.Y" );
        Map( m => m.z ).Name( "upperarm.Z" );
    }
}

public class UpperArmOMap : ClassMap<Position> {
    public UpperArmOMap() {
        Map( m => m.x ).Name( "upperarmO.X" );
        Map( m => m.y ).Name( "upperarmO.Y" );
        Map( m => m.z ).Name( "upperarmO.Z" );
    }
}

public class LeftShoulderMap : ClassMap<Position> {
    public LeftShoulderMap() {
        Map( m => m.x ).Name( "leftShoulder.X" );
        Map( m => m.y ).Name( "leftShoulder.Y" );
        Map( m => m.z ).Name( "leftShoulder.Z" );
    }
}

public class LeftShoulderOMap : ClassMap<Position> {
    public LeftShoulderOMap() {
        Map( m => m.x ).Name( "leftShoulderO.X" );
        Map( m => m.y ).Name( "leftShoulderO.Y" );
        Map( m => m.z ).Name( "leftShoulderO.Z" );
    }
}

public class RightShoulderMap : ClassMap<Position> {
    public RightShoulderMap() {
        Map( m => m.x ).Name( "rightShoulder.X" );
        Map( m => m.y ).Name( "rightShoulder.Y" );
        Map( m => m.z ).Name( "rightShoulder.Z" );
    }
}

public class RightShoulderOMap : ClassMap<Position> {
    public RightShoulderOMap() {
        Map( m => m.x ).Name( "rightShoulderO.X" );
        Map( m => m.y ).Name( "rightShoulderO.Y" );
        Map( m => m.z ).Name( "rightShoulderO.Z" );
    }
}

public class HmdMap : ClassMap<Position> {
    public HmdMap() {
        Map( m => m.x ).Name( "hmd.X" );
        Map( m => m.y ).Name( "hmd.Y" );
        Map( m => m.z ).Name( "hmd.Z" );
    }
}

public class HmdOMap : ClassMap<Position> {
    public HmdOMap() {
        Map( m => m.x ).Name( "hmdO.X" );
        Map( m => m.y ).Name( "hmdO.Y" );
        Map( m => m.z ).Name( "hmdO.Z" );
    }
}