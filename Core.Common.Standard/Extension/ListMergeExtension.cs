using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KY.Core
{
    public static class ListMergeExtension
    {
        public static void Merge<TList, TSource>(this IList<TList> list, IEnumerable<TSource> source, Func<TSource, TList, bool> equalityAction = null)
            where TSource : TList
        {
            Merge(list, source, equalityAction, s => (TList)s);
        }

        public static void Merge<TList, TSource>(this IList<TList> list, IEnumerable<TSource> source, Func<TSource, TList, bool> equalityAction,
                                                 Func<TSource, TList> convertAction)
        {
            List<TList> remainingItems = new List<TList>(list);
            foreach (TSource item in source)
            {
                TList found = remainingItems.Find(item, equalityAction);
                if (Equals(found, default(TList)))
                {
                    // Hinzufügen falls nicht gefunden
                    TList convertedItem = convertAction(item);
                    list.Add(convertedItem);
                }
                else if (found is IMergeable<TSource>)
                {
                    // Mergen wenn möglich
                    (found as IMergeable<TSource>).Merge(item);
                }
                else if (found is IMergeable<TList>)
                {
                    // Mergen wenn möglich
                    TList convertedItem = convertAction(item);
                    (found as IMergeable<TList>).Merge(convertedItem);
                }
                else
                {
                    // Ersetzten falls gefunden
                    int index = list.IndexOf(found);
                    TList convertedItem = convertAction(item);
                    list.Insert(index, convertedItem);
                    list.Remove(found);
                }
                remainingItems.Remove(found);
            }
            // Alle Elemente die nicht in source enthalten waren, müssen entfernt werden
            remainingItems.ForEach(item => list.Remove(item));
        }

        public static TList Find<TList, TSource>(this IList<TList> list, TSource search, Func<TSource, TList, bool> equalityAction = null)
        {
            equalityAction = equalityAction ?? ((s, c) => Equals(s, c));
            foreach (TList compare in list)
            {
                if (equalityAction(search, compare))
                {
                    return compare;
                }
            }
            return default(TList);
        }

        public static void Merge(this IList list, IEnumerable source, Func<object, object> convert = null)
        {
            convert = convert ?? (o => o);
            List<object> remainingItems = list.Cast<object>().ToList();
            foreach (object item in source)
            {
                object found = remainingItems.Find(item.Equals);
                if (found == null)
                {
                    // Hinzufügen falls nicht gefunden
                    list.Add(convert(item));
                }
                else if (found is IMergeable)
                {
                    // Mergen wenn möglich
                    ((IMergeable)found).Merge(convert(item));
                }
                else
                {
                    // Ersetzten falls gefunden
                    int index = list.IndexOf(found);
                    list.Insert(index, convert(item));
                    list.Remove(found);
                }
                remainingItems.Remove(found);
            }
            // Alle Elemente die nicht in source enthalten waren, müssen entfernt werden
            remainingItems.ForEach(list.Remove);
        }
    }
}