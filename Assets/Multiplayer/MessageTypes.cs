using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MessageTypes
{
    public enum MessageType
    {
        _NONE,              //Test

        ACKNOWLEDGEMENTS,
        CONFIRMATION,       //No msg
        START,              //No msg

        POSITION,
        SHOOT,
        KILL,               //No msg

        PAUSE,              //No msg
        UNPAUSE,            //No msg
        RESET,              //No msg

        DISCONNECT,         //No msg

        CHAT,

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
}