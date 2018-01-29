using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Fiddler;
using System.Collections;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示会话集合
    /// </summary>
    public class SessionCollection : IEnumerable<Session>
    {
        private readonly ConcurrentDictionary<int, Session> dictionary = new ConcurrentDictionary<int, Session>();

        /// <summary>
        /// 添加会话
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public bool Add(Session session)
        {
            var state = dictionary.TryAdd(session.id, session);
            if (state == true)
            {
                session.OnStateChanged += (s, e) =>
                {
                    if (e.newState == SessionStates.Aborted)
                    {
                        dictionary.TryRemove(session.id, out session);
                    }
                };
            }
            return state;
        }

        /// <summary>
        /// 迭代会话
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Session> GetEnumerator()
        {
            return this.dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
