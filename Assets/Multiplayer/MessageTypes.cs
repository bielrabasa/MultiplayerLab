using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MessageTypes
{
    public enum MessageType
    {
        _NONE,              //Test

        ACKNOWLEDGEMENTS,   //Extra
        CONFIRMATION,       
        START,              

        POSITION,           //Extra
        SHOOT,              //Extra
        KILL,               

        OBSTACLE,           //Extra

        PAUSE,              
        UNPAUSE,            
        RESET,              

        DISCONNECT,         

        CHAT,               //Extra

        _MESSAGE_TYPE_COUNT //Test
    }

    public class Message
    {
        public Message(MessageType type) => this.type = type;

        public uint id;
        public float time;
        public MessageType type;
    }

    public class Acknowledgements : Message
    {
        public Acknowledgements(List<uint> acks) : base(MessageType.ACKNOWLEDGEMENTS)
        {
            this.acks = acks;
        }

        public List<uint> acks;
    }

    public class Position : Message
    {
        public Position(Vector3 pos, float topRot, float botRot) : base(MessageType.POSITION)
        {
            this.pos = pos;
            this.topRot = topRot;
            this.botRot = botRot;
        }

        public Vector3 pos;
        public float topRot;
        public float botRot;
    }

    public class Shoot : Message
    {
        public Shoot(Vector3 pos, float rot) : base (MessageType.SHOOT)
        {
            this.pos = pos;
            this.rot = rot;
        }

        public Vector3 pos;
        public float rot;
    }

    public class Chat : Message 
    {
        public Chat(string chatMsg) : base(MessageType.CHAT)
        { 
            this.chatMsg = chatMsg;
        }

        public string chatMsg;
    }

    public class Obstacle : Message
    {
        public Obstacle(int idObject) : base(MessageType.OBSTACLE)
        {
            this.idObject = idObject;
        }

        public int idObject;

    }
}