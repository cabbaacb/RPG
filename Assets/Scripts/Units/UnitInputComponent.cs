using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace RPG.Units
{
    public class UnitInputComponent : MonoBehaviour
    {
        private Dictionary<string, FieldInfo> _events = new Dictionary<string, FieldInfo>();
        private Func<string, Delegate[]> _expression;

        protected Vector3 _movement;

        public ref Vector3 MoveDirection() => ref _movement;
        public SimpleHandle MainAttackEventHandler;
        public SimpleHandle TargetEventHandler;
        public SimpleHandle AdditionalAttackEventHandler;
        //public SimpleHandle MeleeSetEventHandler;
        //public SimpleHandle RangeSetEventHandler;

        protected void CallOnAttackEvent() => MainAttackEventHandler?.Invoke();
        protected void CallOnAdditionalAttackEvent() => AdditionalAttackEventHandler?.Invoke();
        protected void CallOnTargetEvent() => TargetEventHandler?.Invoke();
        //protected void CallOnMeleeSetEvent() => MeleeSetEventHandler?.Invoke();
        //protected void CallOnRangeSetEvent() => RangeSetEventHandler?.Invoke();


        protected virtual void Awake()
        {
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(t => t.FieldType == typeof(SimpleHandle));
            foreach (var it in fields)
                _events.Add(it.Name, it);
            
            var strExpr = Expression.Parameter(typeof(string));
            var dicConstExpr = Expression.Constant(_events, typeof(Dictionary<string, FieldInfo>));
            var valueExpr = Expression.Property(dicConstExpr, typeof(Dictionary<string, FieldInfo>).GetProperty("Item"), strExpr);

            var fieldExpr = Expression.Call(valueExpr, typeof(FieldInfo).GetMethod(nameof(FieldInfo.GetValue)), Expression.Constant(this));

            var delegates = Expression.Convert(fieldExpr, typeof(MulticastDelegate));

            var methodInfo = typeof(MulticastDelegate).GetMethod(nameof(MulticastDelegate.GetInvocationList));
            var getInvoExpr = Expression.Call(delegates, methodInfo);

            var arrayExpr = Expression.Convert(getInvoExpr, typeof(Delegate[]));
            _expression = Expression.Lambda<Func<string, Delegate[]>>(arrayExpr, strExpr).Compile();

        }

        protected void CallSimplerHandle(string name)
        {
            //var field = _events[name];
            //var fieldExpr = Expression.Field(Expression.Constant(this), field);

            foreach (var @event in _expression.Invoke(name))
            {
                @event.Method.Invoke(@event.Target, null);
            }

        }


        
    }
}
