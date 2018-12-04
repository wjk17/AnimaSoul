using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Esa.UI
{
    public class UIScorePlayer : MonoBehaviour
    {
        public Slider slider;
        public ScoreReader score;
        public int beatIdx;
        public bool playing = true;

        float timer;
        public float interval = 0.2f;
        List<Vector2Int> list;
        private void Awake()
        {
            if (FindObjectOfType<FretsIK>() == null) enabled = false;
        }
        void DoPlaying()
        {
            var speed = slider.value;
            timer += Time.deltaTime * speed;
            if (timer > interval)
            {
                timer = 0;
                DoMo:
                var note = score[beatIdx];
                DoMotion(note);
                beatIdx++;
                if (beatIdx >= score.beatTotal)
                {
                    playing = false;
                    beatIdx = 0;
                }
                else if (note.Between(Note.H0, Note.H18))
                {
                    goto DoMo;
                }
            }
        }
        void Add(Note a, Note b)
        {
            list.Add(new Vector2Int((int)a, (int)b));
        }
        private void DoHand(int fret)
        {
            var ik = FindObjectOfType<FretsIK>();
            ik.handFret = fret;
        }
        private void DoMotion(Note note)
        {
            if (note.Between(Note.H0, Note.H18))
                DoHand(note - Note.H0);

            var ik = FindObjectOfType<FretsIK>();
            var n = (int)note;

            list = new List<Vector2Int>();
            Add(Note.E0, Note.E18);
            Add(Note.A0, Note.A18);
            Add(Note.D0, Note.D18);
            Add(Note.G0, Note.G18);

            foreach (var range in list)
            {
                if (n.Between(range.x, range.y))
                {
                    ik.chord = n / 19;
                    ik.fret = n - range.x;
                    break;
                }
            }
            os = ik.fret - ik.hand;
            if (os <= -1) ik.finger = 1;
            else if (os <= 0) ik.finger = 2;
            else if (os <= 1) ik.finger = 3;
            else if (os <= 2) ik.finger = 4;
            //ik.finger++;
            //if (ik.finger > 4) ik.finger = 0;
        }
        public int os;
        void Update()
        {
            if (playing)
            {
                DoPlaying();
            }
        }
    }
}