using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        public int playerID;
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

    //SERIALIZER

    public class Serializer
    {
        //From message to Json string
        public static string ToJson(Message m)
        {
            return JsonUtility.ToJson(m);
        }

        //From message to Bytes
        public static byte[] ToBytes(Message m)
        {
            return Encoding.ASCII.GetBytes(ToJson(m));
        }

        //From bytes to message
        public static Message FromBytes(byte[] data, int size)
        {
            return FromJson(Encoding.ASCII.GetString(data, 0, size));
        }

        //From Json string to message
        public static Message FromJson(string json)
        {
            Message m = JsonUtility.FromJson<Message>(json);

            //Check to reDeserialize in case of inherited class
            switch (m.type)
            {
                case MessageType.ACKNOWLEDGEMENTS:
                    {
                        m = JsonUtility.FromJson<Acknowledgements>(json);
                        break;
                    }
                case MessageType.POSITION:
                    {
                        m = JsonUtility.FromJson<Position>(json);
                        break;
                    }
                case MessageType.SHOOT:
                    {
                        m = JsonUtility.FromJson<Shoot>(json);
                        break;
                    }
                case MessageType.CHAT:
                    {
                        m = JsonUtility.FromJson<Chat>(json);
                        break;
                    }
                case MessageType.OBSTACLE:
                    {
                        m = JsonUtility.FromJson<Obstacle>(json);
                        break;
                    }
            }

            return m;
        }
    }
}