using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    public class Trajectory : ICloneable
    {
        List<Node> nodes;

        List<Side> sides;

        Node initial;

        public Trajectory()
        {

            nodes = new List<Node>();
            sides = new List<Side>();
            initial = null;
        }

        public Node addNode(string id, int x, int y, float scale)
        {

            Node node = new Node(id, x, y, scale);
            if (nodes.Contains(node))
            {
                node = nodes[nodes.IndexOf(node)];
            }
            else
            {
                nodes.Add(node);
            }
            if (nodes.Count == 1)
            {
                initial = nodes[0];
            }
            return node;
        }

        public Side addSide(string idStart, string idEnd, int length)
        {

            if (idStart.Equals(idEnd))
                return null;
            Side side = new Side(idStart, idEnd);
            Node a = getNodeForId(idStart);
            Node b = getNodeForId(idEnd);
            if (a != null && b != null)
            {
                int x = a.getX() - b.getX();
                int y = a.getY() - b.getY();
                int realLength = Mathf.RoundToInt(new Vector2(x,y).magnitude);
                if (length == -1)
                    length = realLength;
                
                side.setRealLength(realLength);
                side.setLenght(length);
            }

            if (sides.Contains(side))
            {
                return null;
            }
            else
            {
                sides.Add(side);
            }
            return side;
        }

        public void removeNode(Node node)
        {

            for (int index = 0; index < nodes.Count; index++)
            {
                if (nodes[index].getID().Equals(node.getID()))
                {
                    for (int i = 0; i < sides.Count;)
                    {
                        Side side = sides[i];
                        if (side.getIDEnd().Equals(node.getID()) || side.getIDStart().Equals(node.getID()))
                            sides.RemoveAt(i);
                        else
                            i++;
                    }
                    nodes.Remove(nodes[index]);
                }
            }

            if (initial.getID().Equals(node.getID()))
                initial = null;

            /*if( nodes.contains( node ) ) {
                node = nodes.get( nodes.indexOf( node ) );
                for( int i = 0; i < sides.size( ); ) {
                    Side side = sides.get( i );
                    if( side.getIDEnd( ).Equals( node.getID( ) ) || side.getIDStart( ).Equals( node.getID( ) ) )
                        sides.remove( i );
                    else
                        i++;
                }
            }
            nodes.remove( node );*/
        }

        public void removeNode(int x, int y)
        {

            Node node = new Node("id", x, y, 1.0f);

            foreach (Node n in nodes)
            {
                if (n.getX() == node.getX() && n.getY() == node.getY())
                {
                    for (int i = 0; i < sides.Count;)
                    {
                        Side side = sides[i];
                        if (side.getIDEnd().Equals(n.getID()) || side.getIDStart().Equals(n.getID()))
                            sides.RemoveAt(i);
                        else
                            i++;
                    }
                    nodes.Remove(n);
                    break;
                }
            }

            /*if( nodes.contains( node ) ) {
                node = nodes.get( nodes.indexOf( node ) );
                for( int i = 0; i < sides.size( ); ) {
                    Side side = sides.get( i );
                    if( side.getIDEnd( ).Equals( node.getID( ) ) || side.getIDStart( ).Equals( node.getID( ) ) )
                        sides.remove( i );
                    else
                        i++;
                }
                nodes.remove( node );
            }*/

        }

        public List<Node> getNodes()
        {

            return nodes;
        }

        public List<Side> getSides()
        {

            return sides;
        }

        public class Node : ICloneable
        {

            private string id;

            private int x;

            private int y;

            private float scale;

            public Node(string id, int x, int y, float scale)
            {

                this.id = id;
                this.x = x;
                this.y = y;
                this.scale = scale;
            }

            public string getID()
            {

                return id;
            }

            public int getX()
            {

                return x;
            }

            public int getY()
            {

                return y;
            }

            public float getScale()
            {

                return scale;
            }

            public void setScale(float scale)
            {

                this.scale = scale;
            }

            public Rect getEditorRect(float nodeWidth, float nodeHeight)
            {
                return new Rect(x - 0.5f * nodeWidth * scale, y - 0.5f * nodeHeight * scale, nodeWidth * scale, nodeHeight * scale);
            }

            /*
                    @Override
                        public boolean equals(Object o)
                    {

                        if (o == null)
                            return false;
                        if (o instanceof Node ) {
                            Node node = (Node)o;
                            if (node.id.Equals(this.id) && node.x == this.x && node.y == this.y)
                                return true;
                        }
                        return false;
                    }
                    */

            public void setValues(int x, int y, float scale)
            {

                this.x = x;
                this.y = y;
                this.scale = scale;
            }

            public object Clone()
            {
                Node n = (Node)this.MemberwiseClone();
                // the id mut be unique for each node in the chapter
                n.id = "node" + (new System.Random()).Next(10000);
                n.scale = scale;
                n.x = x;
                n.y = y;
                return n;
            }
        }

        /*
    @Override
        public Object clone() throws CloneNotSupportedException
    {

        Node n = (Node) super.clone( );
        // the id mut be unique for each node in the chapter
        n.id = "node" + ( new Random() ).nextInt( 10000 );
    n.scale = scale;
            n.x = x;
            n.y = y;
            return n;
        }
    }*/

        public class Side : ICloneable
        {

            private readonly string idStart;

            private readonly string idEnd;

            private float length = 1;

            private float realLength = 1;

            public Side(string idStart, string idEnd)
            {

                this.idStart = idStart;
                this.idEnd = idEnd;
            }

            public void setRealLength(float realLength)
            {
                this.realLength = realLength;
            }

            public string getIDStart()
            {

                return idStart;
            }

            public string getIDEnd()
            {

                return idEnd;
            }

            public void setLenght(float length)
            {

                this.length = length;
            }

            public float getLength()
            {

                return length;
            }

            public float getRealLength()
            {
                return realLength;
            }

            public object Clone()
            {
                Side s = (Side)this.MemberwiseClone();
                return s;
            }

            public override bool Equals(object obj)
            {
                var side = obj as Side;
                return side != null && side.idStart == idStart && side.idEnd == idEnd;
            }

            public override int GetHashCode()
            {
                return idStart.GetHashCode() + idEnd.GetHashCode();
            }
        }

        public Node getNodeForId(string id)
        {

            if (id == null)
                return null;
            foreach (Node node in nodes)
            {
                if (id.Equals(node.getID()))
                    return node;
            }
            return null;
        }

        public void setInitial(string id)
        {

            initial = getNodeForId(id);
        }

        public Node getInitial()
        {

            return initial;
        }

        public void deleteUnconnectedNodes()
        {
            List<Node> connected = new List<Node>();
            if (initial == null)
            {
                if (nodes.Count > 0)
                    initial = nodes[0];
            }
            connected.Add(initial);
            int i = 0;
            while (i < connected.Count)
            {
                Node temp = connected[i];
                i++;
                foreach (Side side in sides)
                {
                    if (side.getIDEnd().Equals(temp.getID()))
                    {
                        Node node = this.getNodeForId(side.getIDStart());
                        if (node != null && !connected.Contains(node))
                            connected.Add(node);
                    }
                    if (side.getIDStart().Equals(temp.getID()))
                    {
                        Node node = this.getNodeForId(side.getIDEnd());
                        if (node != null && !connected.Contains(node))
                            connected.Add(node);
                    }
                }
            }
            i = 0;
            while (i < nodes.Count)
            {
                if (!connected.Contains(nodes[i]))
                {
                    int j = 0;
                    while (j < sides.Count)
                    {
                        if (sides[j].getIDEnd().Equals(nodes[i].getID()))
                            sides.RemoveAt(j);
                        else if (sides[j].getIDStart().Equals(nodes[i].getID()))
                            sides.RemoveAt(j);
                        else
                            j++;
                    }
                    nodes.RemoveAt(i);
                }
                else
                    i++;
            }
        }

        public object Clone()
        {
            Trajectory t = (Trajectory)this.MemberwiseClone();
            Dictionary<string, string> keyRelationship = new Dictionary<string, string>();
            string initialId = initial.getID();
            t.initial = (initial != null ? (Node)initial.Clone() : null);
            keyRelationship.Add(initialId, t.initial.getID());
            if (nodes != null)
            {
                t.nodes = new List<Node>();
                foreach (Node n in nodes)
                {
                    if (n.getID().Equals(initialId))
                        t.nodes.Add(t.initial);
                    else
                    {
                        string oldId = n.getID();
                        //node clone generates a new Id
                        Node newNode = (Node)n.Clone();
                        t.nodes.Add(newNode);
                        keyRelationship.Add(oldId, newNode.getID());
                    }
                }
            }
            if (sides != null)
            {
                t.sides = new List<Side>();
                foreach (Side s in sides)
                {
                    //Side clone does not generate a new Id
                    Side newSide = new Side(keyRelationship[s.getIDStart()], keyRelationship[s.getIDEnd()]);
                    newSide.setLenght(s.getLength());
                    newSide.setRealLength(s.getRealLength());
                    t.sides.Add(newSide);

                }
            }
            return t;
        }

        /*
    @Override
       public Object clone() throws CloneNotSupportedException
    {
       Trajectory t = (Trajectory) super.clone( );
       HashMap<string,string> keyRelationship = new HashMap<string, string>();
           string initialId = initial.getID();
    t.initial = ( initial != null ? (Node) initial.clone( ) : null );
           keyRelationship.put( initialId , t.initial.getID( ) );
           if( nodes != null ) {
               t.nodes = new List<Node>( );
               for( Node n : nodes ) {
                   if( n.getID( ).Equals(initialId ) )
                       t.nodes.add( t.initial );
                   else{
                       string oldId = n.getID();
    //node clone generates a new Id
    Node newNode = (Node)n.clone();
    t.nodes.add( newNode);
                       keyRelationship.put(oldId, newNode.getID( ));
                   }
               }
           }
           if( sides != null ) {
               t.sides = new List<Side>( );
               for( Side s : sides ){
                   //Side clone does not generate a new Id
                   Side newSide = new Side(keyRelationship.get(s.getIDStart()), keyRelationship.get(s.getIDEnd()));
    newSide.setLenght( s.getLength( ) );
                   newSide.setRealLength( s.getRealLength( ) );
                   t.sides.add(  newSide );

               }
           }
           return t;
       }
       */
    }
}