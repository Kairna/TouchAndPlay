using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TouchAndPlay.utils;
using TouchAndPlay.effects;

namespace TouchAndPlay.bubbles
{
    class DragBubble
    {
        private DragBubbleType type;
        private Dictionary<BubbleState, Texture2D> handTextureList;
        private Texture2D lineTexture;

        public Bubble bubble1;
        private Bubble bubble2;

        private int lockCount;
        private int lockCtr;

        private int popCount;
        private int popCtr;

        private float lineAlpha;
        private float radianAngle;
        private Rectangle lineRectangle;

        public DragBubbleState currentState;

        public float xSpeedUnit;
        public float ySpeedUnit;
        public float speed;

        EffectHandler effectHandler;

        public DragBubble(DragBubbleType type, Dictionary<BubbleState, Texture2D> textureList, Texture2D lineTexture, EffectHandler effectHandler, float minDistance = 50)
        {
            this.type = type;
            this.currentState = DragBubbleState.APPEARING;
            this.handTextureList = textureList;
            this.lineTexture = lineTexture;
            this.lineAlpha = GameConfig.LINE_ALPHA;
            this.lockCount = GameConfig.LOCK_COUNT;
            this.popCount = GameConfig.DRAG_BUBBLE_POPCOUNT;
            this.effectHandler = effectHandler;

            createSet(minDistance);
        }

        public DragBubble(DragBubbleType type, Dictionary<BubbleState, Texture2D> textureList, Texture2D lineTexture, EffectHandler effectHandler, Vector2 pos1, Vector2 pos2)
        {
            this.type = type;
            this.currentState = DragBubbleState.APPEARING;
            this.handTextureList = textureList;
            this.lineTexture = lineTexture;
            this.lineAlpha = GameConfig.LINE_ALPHA;
            this.lockCount = GameConfig.LOCK_COUNT;
            this.popCount = GameConfig.DRAG_BUBBLE_POPCOUNT;
            this.effectHandler = effectHandler;

            //create first hand bubble
            bubble1 = new Bubble(pos1.X, pos1.Y, handTextureList, BubbleType.HAND);
            bubble1.setStayingDuration(GameConfig.DRAGBUBBLE_DURATION);

            //create second hand bubble
            bubble2 = new Bubble((int)pos2.X, pos2.Y, handTextureList, BubbleType.INACTIVE_STATIC_HAND);
            bubble2.setStayingDuration(GameConfig.DRAGBUBBLE_DURATION);

            radianAngle = MathUtil.getPIAngle(bubble1.pos, bubble2.pos);
            float width = MathUtil.getDistance(bubble1.pos, bubble2.pos);
            lineRectangle = new Rectangle(0, 0, (int)width, 13);

            xSpeedUnit = (float)Math.Cos(radianAngle);
            ySpeedUnit = (float)Math.Sin(radianAngle);
        }

        private void createSet(float minDistance)
        {
            switch (type)
            {
                case DragBubbleType.HAND:
                    Vector2 randomPoint;

                    //create first hand bubble
                    randomPoint = Randomizer.createRandomPoint();
                    bubble1 = new Bubble((int)randomPoint.X, (int)randomPoint.Y, handTextureList, BubbleType.HAND);
                    //bubble1 = new Bubble(100, 100, handTextureList, BubbleType.HAND);
                    bubble1.setStayingDuration(GameConfig.DRAGBUBBLE_DURATION);

                    //create second hand bubble
                    double randomAngle = Randomizer.randomRadian();
                    int randDistance = Randomizer.random((int)minDistance, (int)minDistance);
                    //randomPoint = new Vector2((float)Math.Cos(randomAngle)*randDistance, (float)Math.Sin(randomAngle)*randDistance);
                    randomPoint = Randomizer.createRandomPoint();
                    bubble2 = new Bubble((int)randomPoint.X, (int)randomPoint.Y, handTextureList, BubbleType.INACTIVE_STATIC_HAND);
                    //bubble2 = new Bubble(400, 400, handTextureList, BubbleType.INACTIVE_STATIC_HAND);
                    bubble2.setStayingDuration(GameConfig.DRAGBUBBLE_DURATION);

                    radianAngle = MathUtil.getPIAngle(bubble1.pos, bubble2.pos);
                    float width = MathUtil.getDistance(bubble1.pos, bubble2.pos);
                    lineRectangle = new Rectangle(0, 0, (int)width, 13);

                    xSpeedUnit = (float) Math.Cos(radianAngle);
                    ySpeedUnit = (float) Math.Sin(radianAngle);
                    break;

            }

        }

        private void DrawDirectionLine(SpriteBatch sprite)
        {
            sprite.Draw(lineTexture, bubble1.pos, lineRectangle, Color.White * lineAlpha, radianAngle, Vector2.One*5, Vector2.One, SpriteEffects.None, 0);

        }

        public void resetPopCounters()
        {
            bubble1.resetPopCounter();
            bubble2.resetPopCounter();

        }

        public bool isWithinRadianRange(Vector2 point)
        {
            return Math.Abs(MathUtil.getPIAngle(bubble1.pos, point) - radianAngle) <= GameConfig.DRAG_BUBBLE_RADIAN_RANGE;
        }

        public void moveForward(float speed)
        {
            this.speed = speed;
            
        }


        public void Update()
        {
            updateAllBubbles();
            updateState();
            resetPopCounters();
            updateSpeed();
        }

        private void updateSpeed()
        {
            if (speed != 0)
            {
                bubble1.xPos = bubble1.pos.X + xSpeedUnit*speed;
                bubble1.yPos = bubble1.pos.Y + ySpeedUnit*speed;
                bubble1.pos = new Vector2(bubble1.xPos, bubble1.yPos);

                lineRectangle = new Rectangle(0, 0, (int)MathUtil.getDistance(bubble1.pos, bubble2.pos), lineTexture.Height);
            }
        }

        private void updateState()
        {
            switch (currentState)
            {
                case DragBubbleState.APPEARING:
                    if (bubble1.currentState == BubbleState.NORMAL_STATE)
                    {
                        currentState = DragBubbleState.NORMAL;
                    }
                    break;
                case DragBubbleState.NORMAL:
                    lockCtr = 0;
                    break;
                case DragBubbleState.HOVERED:
                    lockCtr++;
                    if (lockCtr >= lockCount)
                    {
                        currentState = DragBubbleState.LOCKED_IN;
                        bubble1.setState(BubbleState.LOCKED_IN);
                    }
                    break;
                case DragBubbleState.LOCKED_IN:
                    //do something

                    break;
                case DragBubbleState.POP_STATE:
                case DragBubbleState.DISAPPEARING:
                    lineAlpha -= 0.020f;
                    if (lineAlpha <= 0f)
                    {
                        currentState = DragBubbleState.REMOVAL_STATE;
                    }
                    break;
            }
        }

        private void updateAllBubbles()
        {
            bubble1.Update();
            bubble2.Update();

            if (bubble1.currentState == BubbleState.DISAPPEARING)
            {
                currentState = DragBubbleState.DISAPPEARING;
            }
            else if (bubble2.currentState == BubbleState.HIGHLIGHTED_STATE)
            {
                popCtr += 1;
                if (popCtr >= popCount)
                {
                    currentState = DragBubbleState.POP_STATE;
                    bubble1.setState(BubbleState.POP_STATE);
                    bubble2.setState(BubbleState.POP_STATE);
                    effectHandler.addEffect((int)bubble1.xPos, (int)bubble1.yPos, 20);
                    effectHandler.addEffect((int)bubble1.xPos, (int)bubble1.yPos, 10, Color.Yellow);
                    effectHandler.addEffect((int)bubble1.xPos, (int)bubble1.yPos, 5, Color.White);
                }
            }
            else
            {
                popCtr = 0;
            }

        }


        public void Draw(SpriteBatch sprite)
        {

            DrawDirectionLine(sprite);

            
            bubble2.Draw(sprite);
            bubble1.Draw(sprite);

        }


        internal void deccelerate()
        {
            speed = 0;

        }

        public void follow(Vector2 currentHand)
        {
            bubble1.pos = currentHand;

            bubble1.xPos = currentHand.X;
            bubble1.yPos = currentHand.Y;

            radianAngle = MathUtil.getPIAngle(bubble1.pos, bubble2.pos);
            lineRectangle = new Rectangle(0, 0, (int)MathUtil.getDistance(bubble1.pos, bubble2.pos), lineTexture.Height);
            bubble1.collisionBox.X = (int)bubble1.xPos - bubble1.radius;
            bubble1.collisionBox.Y = (int)bubble1.yPos - bubble1.radius;

            if (isColliding(bubble1, bubble2) && MathUtil.getDistance(bubble1.pos, bubble2.pos) <= 20)
            {
                bubble1.pos = bubble2.pos;
                bubble1.xPos = bubble2.pos.X;
                bubble1.yPos = bubble2.pos.Y;
                bubble2.setState(BubbleState.HIGHLIGHTED_STATE);
                bubble1.alpha = 0.5f;
                
            }
            else
            {
                bubble2.setState(BubbleState.STATIC_INACTIVE);
                bubble1.alpha = 1;
            }
        }

        private bool isColliding(Bubble bubble1, Bubble bubble2)
        {
            return bubble1.collisionBox.Intersects(bubble2.collisionBox);
        }
    }
}
