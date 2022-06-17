﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Source: https://github.com/mondrasovic/UnionFind
namespace UnionFind {
    class Node<T>
        where T : IEqualityComparer<T>
    {
        public T Data { get; set; }
        public Node<T> Parent { get; set; }
        public int Rank { get; set; }

        public Node(T data)
        {
            Data = data;
            Parent = this;
            Rank = 0;
        }
    }

    public class DisjointSet<T> : IEnumerable<T>
        where T : IEqualityComparer<T>
    {
        Dictionary<T, Node<T>> nodes;

        public int Count { get { return nodes.Count; } }

        public DisjointSet()
        {
            nodes = new Dictionary<T, Node<T>>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return nodes.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return nodes.Keys.GetEnumerator();
        }

        public bool ContainsData(T data)
        {
            return nodes.ContainsKey(data);
        }

        public bool MakeSet(T data)
        {
            if (ContainsData(data))
                return false;

            nodes.Add(data, new Node<T>(data));
            return true;
        }

        public bool Union(T dataA, T dataB)
        {
            var nodeA = nodes[dataA];
            var nodeB = nodes[dataB];

            var parentA = nodeA.Parent;
            var parentB = nodeB.Parent;

            if (parentA == parentB)
                return false;

            if (parentA.Rank >= parentB.Rank)
            {
                if (parentA.Rank == parentB.Rank)
                    ++parentA.Rank;

                parentB.Parent = parentA;
            }
            else
            {
                parentA.Parent = parentB;
            }

            return true;
        }

        public T FindSet(T data)
        {
            return FindSet(nodes[data]).Data;
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public void Clear()
        {
            nodes.Clear();
        }

        public Dictionary<T, HashSet<T>> GetClusters() {
            Dictionary<T, HashSet<T>> clusters = new Dictionary<T, HashSet<T>>();

            foreach ((T key, Node<T> value) in nodes) {
                T representative = FindSet(key);

                if (!clusters.ContainsKey(representative)) {
                    clusters.Add(representative, new HashSet<T>());
                    clusters[representative].Add(representative);
                }

                clusters[representative].Add(key);
            }

            return clusters;
        }

        Node<T> FindSet(Node<T> node)
        {
            var parent = node.Parent;
            if (parent == node)
                return node;

            node.Parent = FindSet(node.Parent);
            return node.Parent;
        }
    }
}