﻿using System.Collections.Generic;

namespace CommonTypes
{
    public interface IServer
    {
        List<Meeting> GetMeetings(VectorClock vector, List<Meeting> clientMeetings);

        void CreateMeeting(VectorClock vector, Meeting m);

        void RBCreateMeeting(string sender_url, VectorClock vector, Meeting m);

        void JoinMeeting(string user, VectorClock vector, string meetingTopic, List<Slot> slots);

        void RBJoinMeeting(string sender_url, VectorClock vector, string user, string meetingTopic, List<Slot> slots);

        void CloseMeeting(VectorClock vector, string user, string meetingTopic);

        void RBCloseMeeting(string sender_url, VectorClock vector, string meetingTopic);

        void RBCloseTicket(string sender_url, string topic, int seq);

        void AddRoom(string location, int capacity, string room_name);

        void ServerCrash(string crash_url);

        void RBServerCrash(string sender_url, string crash_url);

        void Status();
        /*
         * Debugging Commands
         */
        void Crash();
        void Freeze();
        void Unfreeze();
        /*
         * Client Management Commands
         */
        Dictionary<string, string> GetClients();
        List<string> GetGossipClients(List<string> vetos, Meeting m);
        VectorClock UpdateVectorClock(VectorClock vector);
        void RegisterClient(string username, string client_url);
        void UnregisterClient(string username);
        /*
         * Leader Election
         */
        void Election();
        void Elected(string leader);
        /*
         * Sequence Commands
         */
        int GetTicket(string topic);
        string GetAlternativeServer();
        void Ping();
    }
}
