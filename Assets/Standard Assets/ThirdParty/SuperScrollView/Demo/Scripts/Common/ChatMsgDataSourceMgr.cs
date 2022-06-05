using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public enum MsgTypeEnum
    {
        Str = 0,
        Picture,
        Count,
    }


    public class PersonInfo
    {
        public int mId;
        public string mName;
        public string mHeadIcon;
    }

    public class ChatMsg
    {
        public int mPersonId;
        public MsgTypeEnum mMsgType;
        public string mSrtMsg;
        public string mPicMsgSpriteName;
    }

    public class ChatMsgDataSourceMgr : MonoBehaviour
    {
        Dictionary<int, PersonInfo> mPersonInfoDict = new Dictionary<int, PersonInfo>();
        List<ChatMsg> mChatMsgList = new List<ChatMsg>();
        static ChatMsgDataSourceMgr instance = null;
        static string[] mChatDemoStrList = {
            "Support ListView and GridView.",
            "Support Infinity Vertical and Horizontal ScrollView.",
            "Support items in different sizes such as widths or heights. Support items with unknown size at init time.",
            "Support changing item count and item size at runtime. Support looping items such as spinners. Support item padding.",
            "Use only one C# script to help the UGUI ScrollRect to support any count items with high performance.",
            };

        static string[] mChatDemoPicList = {
            "grid_pencil_128_g2",
            "grid_flower_200_3",
            "grid_pencil_128_g3",
            "grid_flower_200_7",
            };

        public static ChatMsgDataSourceMgr Get
        {
            get
            {
                if (instance == null)
                {
                    instance = Object.FindObjectOfType<ChatMsgDataSourceMgr>();
                }
                return instance;
            }

        }

        void Awake()
        {
            Init();
        }


        public PersonInfo GetPersonInfo(int personId)
        {
            PersonInfo ret = null;
            if(mPersonInfoDict.TryGetValue(personId, out ret))
            {
                return ret;
            }
            return null;
        }

        public void Init()
        {
            mPersonInfoDict.Clear();
            PersonInfo tInfo = new PersonInfo();
            tInfo.mHeadIcon = "grid_pencil_128_g8";
            tInfo.mId = 0;
            tInfo.mName = "Jaci";
            mPersonInfoDict.Add(tInfo.mId, tInfo);

            tInfo = new PersonInfo();
            tInfo.mHeadIcon = "grid_pencil_128_g5";
            tInfo.mId = 1;
            tInfo.mName = "Toc";
            mPersonInfoDict.Add(tInfo.mId, tInfo);

            InitChatDataSource();

        }

        public ChatMsg GetChatMsgByIndex(int index)
        {
            if (index < 0 || index >= mChatMsgList.Count)
            {
                return null;
            }
            return mChatMsgList[index];
        }

        public int TotalItemCount
        {
            get
            {
                return mChatMsgList.Count;
            }
        }

        void InitChatDataSource()
        {
            mChatMsgList.Clear();
            int count = mChatDemoStrList.Length;
            int count1 = mChatDemoPicList.Length;
            for (int i = 0; i < 100; ++i)
            {
                ChatMsg tMsg = new ChatMsg();
                tMsg.mMsgType = (MsgTypeEnum)(Random.Range(0, 99) % 2); ;
                tMsg.mPersonId = Random.Range(0, 99) % 2;
                tMsg.mSrtMsg = mChatDemoStrList[Random.Range(0, 99) % count];
                tMsg.mPicMsgSpriteName = mChatDemoPicList[Random.Range(0, 99) % count1];
                mChatMsgList.Add(tMsg);
            }
        }

        public void AppendOneMsg()
        {
            int count = mChatDemoStrList.Length;
            int count1 = mChatDemoPicList.Length;
            ChatMsg tMsg = new ChatMsg();
            tMsg.mMsgType = (MsgTypeEnum)(Random.Range(0, 99) % 2); ;
            tMsg.mPersonId = Random.Range(0, 99) % 2;
            tMsg.mSrtMsg = mChatDemoStrList[Random.Range(0, 99) % count];
            tMsg.mPicMsgSpriteName = mChatDemoPicList[Random.Range(0, 99) % count1];
            mChatMsgList.Add(tMsg);
        }

    }

}