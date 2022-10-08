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
        private Func<FieldInfo, Delegate[]> _expression;

        protected Vector3 _movement;

        public ref Vector3 MoveDirection() => ref _movement;
        public SimpleHandle MainAttackEventHandler;
        public SimpleHandle TargetEventHandler;
        public SimpleHandle AdditionalAttackEventHandler;

        protected void CallOnAttackEvent() => MainAttackEventHandler?.Invoke();
        protected void CallOnAdditionalAttackEvent() => AdditionalAttackEventHandler?.Invoke();
        protected void CallOnTargetEvent() => TargetEventHandler?.Invoke();


        protected virtual void Awake()
        {
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(t => t.FieldType == typeof(SimpleHandle));
            foreach (var it in fields)
                _events.Add(it.Name, it);

            var strExpr = Expression.Parameter(typeof(string));
            var dicExpr = Expression.Constant(_events, typeof(Dictionary<string, FieldInfo>));


            var field = _events[name];
            var fieldExpr = Expression.Field(Expression.Constant(this), field);

            var delegates = Expression.Convert(fieldExpr, typeof(MulticastDelegate));

            var methodInfo = typeof(MulticastDelegate).GetMethod(nameof(MulticastDelegate.GetInvocationList));
            var getInvoExpr = Expression.Call(delegates, methodInfo);

            var arrayExpr = Expression.Convert(getInvoExpr, typeof(Delegate[]));
            _expression = Expression.Lambda<Func<FieldInfo, Delegate[]>>(arrayExpr, Expression.Parameter(typeof(FieldInfo))).Compile();


        }

        protected void CallSimplerHandle(string name)
        {            
            foreach(var @event in _expression.Invoke(_events[name]))
            {
                @event.Method.Invoke(@event.Target, null);
            }

        }


        
    }
}
