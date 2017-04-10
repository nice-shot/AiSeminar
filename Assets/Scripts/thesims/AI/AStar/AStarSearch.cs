using System;
using System.Collections.Generic;
using Infra;
using Infra.Collections;
using Priority_Queue;

namespace Ai.AStar {
/// <summary>
/// Implements a generic A* search.
/// </summary>
public static class AStarSearch {
    // This seems like enough...
    private const int MAX_FRINGE_NODES = 2000;

    /// <summary>
    /// A* forward search for a path that reaches the given goal.
    /// </summary>
    /// <returns>Returns null if a path could not be found, or a list of the
    /// transitions that must be followed, in order.</returns>
    public static Queue<ITransition> Search(
            ISearchContext agent,
            IState worldState,
            IGoal goal,
            bool reversePath = false) {
        // This is used to get the nodes from the state we're currently exploring.
        var exploredNodes = new Dictionary<IState, Node>(worldState.GetComparer());
        var closedSet = new HashSet<IState>(worldState.GetComparer());
        var openSet = new FastPriorityQueue<Node>(MAX_FRINGE_NODES);

        var currentNode = Node.Borrow(null, 0f, worldState, null);

        openSet.Enqueue(currentNode, 0f);

        IState nextState;
        Node nextNode;
        while (openSet.Count > 0) {
            // Examine the next node.
            currentNode = openSet.Dequeue();

            // Check if the node satisfies the goal.
            if (currentNode.state.IsGoalState(goal, false)) {
                DebugUtils.Log("Selected path with cost: " + currentNode.Score + " Visited nodes: " + (closedSet.Count + openSet.Count));
                var plan = UnwrapPath(currentNode, reversePath);
                // Return all nodes.
                Node.ReturnAll();

                // Monitor nodes pool size to see if there are problems in the search.
                Node.ReportLeaks();

                return plan;
            }

            // Mark this node as closed - don't explore it again.
            closedSet.Add(currentNode.state);
            exploredNodes.Remove(currentNode.state);

            // Go over all possible transitions.
            var possibleTransitions = currentNode.state.GetPossibleTransitions(agent);
            foreach (var transition in possibleTransitions) {
                var cost = transition.CalculateCost(currentNode.state);
                // Apply the transition to get the next state.
                nextState = transition.ApplyToState(currentNode.state);

                //DebugUtils.LogError("Checking transition: " + currentNode.state + " -(" + transition + ")-> " + nextState);
                //UnityEngine.Debug.Break();

                // Now that we have the new state, we can check if it was
                // already evaluated and if so, we ignore it. We can ignore it
                // thanks to the priority queue - we always check the state whose
                // path to the starting state is the cheapest and so all the next
                // states can only add to this cost.
                if (closedSet.Contains(nextState)) {
                    transition.ReturnSelf();
                    nextState.ReturnSelf();
                    continue;
                }

                float newRunningCost = currentNode.runningCost + cost;
                // Check if we explored this node in the past.
                if (!exploredNodes.TryGetValue(nextState, out nextNode)) {
                    // Found a new node.
                    nextNode = Node.Borrow(currentNode, newRunningCost, nextState, transition);
                    // Cache the node for later.
                    exploredNodes[nextState] = nextNode;

                    DebugUtils.Assert(!openSet.Contains(nextNode), "Open set contains new node... How can this be?");

                    // This is a new node.
                    nextNode.CalculateHeuristicCost(agent, goal);
                    openSet.Enqueue(nextNode, nextNode.Score);
                    //DebugUtils.LogError("New node cost: " + newRunningCost + " " + nextNode.PathToString());
                } else if (newRunningCost < nextNode.runningCost) {
                    // Found the best path so far to this node.
                    nextNode.parent = currentNode;
                    nextNode.runningCost = newRunningCost;
                    nextNode.transition.ReturnSelf();
                    nextNode.transition = transition;
                    nextState.ReturnSelf();
                    //DebugUtils.LogError("!!! Best cost: " + newRunningCost + " " + nextNode.PathToString());
                } else {
                    // Transition is not needed as it is known to be sub optimal.
                    transition.ReturnSelf();
                    nextState.ReturnSelf();
                }
            }
        }
        DebugUtils.Log("NO PLAN. Closed nodes: " + closedSet.Count);
        Node.ReturnAll();
        return null;
    }

    /// <summary>
    /// Unwraps the path going back to the start node.
    /// </summary>
    /// <param name="endNode">The node at the end of the path.</param>
    /// <param name="reverse">Should the path be in reverse order (from the end
    /// to the start).</param>
    private static Queue<ITransition> UnwrapPath(Node endNode, bool reverse) {
        var result = new List<ITransition>();
        var node = endNode;
        while (node.parent != null) {
            result.Add(node.transition.Clone());
            node = node.parent;
        }
        if (!reverse) {
            // The result is already reversed, so reverse it only if we want the
            // path to be from start to end.
            result.Reverse();
        }
        return new Queue<ITransition>(result);
    }


    /// <summary>
    /// Used for building up the graph and holding the running costs of actions.
    /// </summary>
    private class Node : FastPriorityQueueNode, IPoolableObject {
        private static ObjectPool<Node> pool = new ObjectPool<Node>(150, 25);
        private static int lastPoolSize = 150;

        public Node parent;
        /// <summary>
        /// Cost to reach this node.
        /// </summary>
        public float runningCost;
        public float cachedHeuristicCost = float.NegativeInfinity;
        public IState state;
        public ITransition transition;

        public float Score {
            get {
                return runningCost + cachedHeuristicCost;
            }
        }

        /// <summary>
        /// Prints an error everytime the pool size increases. If this happens
        /// regularly, we have a leak.
        /// </summary>
        public static void ReportLeaks() {
            var poolSize = pool.Count;
            if (poolSize > lastPoolSize) {
                DebugUtils.LogError("Nodes pool size: " + poolSize);
                lastPoolSize = poolSize;
            }
        }

        /// <summary>
        /// Use this method to get a new instance. Don't instantiate this class
        /// directly.
        /// </summary>
        public static Node Borrow(Node parent, float runningCost, IState state, ITransition transition) {
            var node = pool.Borrow();
            node.parent = parent;
            node.runningCost = runningCost;
            node.cachedHeuristicCost = float.NegativeInfinity;
            node.state = state;
            node.transition = transition;
            return node;
        }

        public static void ReturnAll() {
            pool.ReturnAll();
        }

        public void ReturnSelf() {
            if (transition != null) {
                transition.ReturnSelf();
                transition = null;
            }
            if (state != null) {
                state.ReturnSelf();
                state = null;
            }
            pool.Return(this);
        }

        /// <summary>
        /// Return an estimated cost to reach the goal from the current state.
        /// </summary>
        public float CalculateHeuristicCost(ISearchContext agent, IGoal goal, bool useCachedValue = true) {
            if (useCachedValue && cachedHeuristicCost >= 0f) {
                return cachedHeuristicCost;
            }
            cachedHeuristicCost = state.CalculateHeuristicCost(agent, goal);
            return cachedHeuristicCost;
        }

        public string PathToString() {
            var node = this;
            var s = string.Empty;
            while (node.parent != null) {
                s = node.transition + ">" + s;
                node = node.parent;
            }
            return s;
        }
    }
}
}
