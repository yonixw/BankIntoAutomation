using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MonyDataMacro
{


    public abstract class StateMachine
    {
        public int State;
        public readonly string StateDesc;
        public abstract void InitMachine();
        
        public State _current = null;
    }

    public class State
    {
        public int id = -1;
        public StateMachine parent = null;
        public string Description = "none";

        public delegate void NewEventSet(State newState);
        static public event NewEventSet StateChanged;

        State(int _id)
        {
            id = _id;
        }

        public void Set()
        {
            if (parent != null)
                parent._current = id;

            if (StateChanged != null)
                StateChanged(this);
        }

        public static implicit operator State(Int32 a) // converting int to State
        {
            return new State(a);
        }

        public static explicit operator Int32(State a) // State to int
        {
            return a.id;
        }
   
        public static bool operator==(State a,State b)
        {
            return a.id == b.id;
        }

        public static bool operator!=(State a, State b)
        {
            return a.id != b.id;
        }

    }
   
    public class WebFSM : StateMachine
    {

        public State IDLE                                   ;
        public State USERNAME_CLICK                         ;
        public State USERNAME_CLICKED                       ;
        public State PASS_CLICK                             ;
        public State PASS_CLICKED                           ;
        public State LOGIN_NAVIGATED                        ;

        public override void InitMachine()
        {
            int counter = 0;
            foreach(FieldInfo fi in typeof(WebFSM).GetFields() ) 
                // Currently works by same order of declaration but doesnt matter - dont assume id are going up
            {
                if ( fi.FieldType.Name == typeof(State).Name)
                {
                    fi.SetValue(this, (State)counter);

                    State current = (State)fi.GetValue(this);
                    current.Description = fi.Name;
                    current.parent = this;

                    if (counter == 0)
                        _current = current;

                    counter++;
                }
            }

            // Set to first
            
        }

        public new int State
        {
            get { return _current.id; }
            set { _current.id = value; }
        }

        public new string StateDesc
        {
            get { return _current.Description; }
        }
    }
}
