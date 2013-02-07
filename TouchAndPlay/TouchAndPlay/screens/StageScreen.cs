using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TouchAndPlay.bubbles;
using Microsoft.Kinect;
using TouchAndPlay.utils;
using Microsoft.Xna.Framework;
using TouchAndPlay.userinterface;
using TouchAndPlay.effects;

namespace TouchAndPlay.screens
{
    class StageScreen:BasicScreen
    {
        private KinectManager kinector;

        //arrays
        private List<Bubble> bubblesOnScreen;
        private List<BubbleSet> bubbleSetsOnScreen;
        private List<DragBubble> dragBubblesOnScreen;
        private Dictionary<BubbleState, Texture2D> blueHands;

        //textures
        private Texture2D lineTexture;
        private Texture2D directionLineTexture;
        private Texture2D dot;

        //iterators
        private int iter1;
        private int iter2;
        private int iter3;

        //intervals and counters
        private int bubbleSetInterval;
        private int bubbleSetCtr;
        private int bubbleInterval;
        private int bubbleCtr;
        private int dragBubbleInterval;
        private int dragBubbleCtr;
        private int msgCtr;
        private int resumeCtr;


        //reference bubble, for referencing purposes only
        private Bubble refBub;

        //states
        private StageScreenStates currentState;
        private StagePreparationStates currentPreparationState;
        private GameSequentialSolo gameMode;

        //game
        private TAPGame currentGame;

        //effect hanlder
        private EffectHandler effectHandler;


        //UI handler
        private MyUI myUI;
        private SpriteFont basicFont;

       
        //target number of bubbles to pop
        private int soloBubblesToPop;
        private int setBubblesToPop;
        private int dragBubblesToPop;

        //game booleans
        private bool usingMouseAsInput;
        private bool allHighlighted;

        //for randomizing points, these are only references. DO NOT INSTANTIATE
        private List<Vector2> rightReachablePoints;
        private List<Vector2> leftReachablePoints;

        private float rightLimbEstSize;
        private int rightLimbNCount;


        private float leftLimbEstSize;
        private int leftLimbNCount;

        /* ===================================================================
         * CONSTRUCTORS
         * ===================================================================
         */
        public StageScreen(KinectManager kinector)
        {
            this.kinector = kinector;

            base.Initialize();
            this.Init();
        }

        public void Init()
        {
            bubblesOnScreen = new List<Bubble>();
            bubbleSetsOnScreen = new List<BubbleSet>();
            dragBubblesOnScreen = new List<DragBubble>();

            //interval between most recently added bubble to the next bubble
            this.bubbleSetInterval = GameConfig.BUBBLE_SET_INTERVAL;
            this.bubbleInterval = GameConfig.SOLO_BUBBLE_INTERVAL;
            this.dragBubbleInterval = GameConfig.DRAG_BUBBLE_INTERVAL;

            this.gameMode = GameSequentialSolo.SOLO_BUBBLES;

            this.soloBubblesToPop = GameConfig.SOLO_BUBBLES_TO_POP;
            this.dragBubblesToPop = GameConfig.DRAG_BUBBLES_TO_POP;
            this.setBubblesToPop = GameConfig.BUBBLE_SETS_TO_POP;

            bubbleCtr = GameConfig.SOLO_BUBBLE_INTERVAL;
            bubbleSetCtr = GameConfig.BUBBLE_SET_INTERVAL;
            dragBubbleCtr = GameConfig.DRAG_BUBBLE_INTERVAL;

            this.effectHandler = new EffectHandler();
            this.usingMouseAsInput = true;

            this.currentGame = TAPGame.SEQUENTIAL_MIXED;

            myUI = new MyUI();

            this.currentState = StageScreenStates.PREPARING;
            
        }

        /* ===================================================================
         * CONTENT LOADER
         * ===================================================================
         */
        public override void LoadContent( ContentManager content )
        {
            base.LoadBasicContent(content);
            //bubbles
            blueHands = new Dictionary<BubbleState, Texture2D>();

            blueHands[BubbleState.NORMAL_STATE] = content.Load<Texture2D>("bubbles/bubble_hand_normal");
            blueHands[BubbleState.HIGHLIGHTED_STATE] = content.Load<Texture2D>("bubbles/bubble_hand_highlighted");
            blueHands[BubbleState.STATIC_INACTIVE] = content.Load<Texture2D>("bubbles/bubble_hand_inactive");
            blueHands[BubbleState.LOCKED_IN] = content.Load<Texture2D>("bubbles/bubble_hand_locked");

            //lines
            lineTexture = content.Load<Texture2D>("lines/line");
            directionLineTexture = content.Load<Texture2D>("lines/directionLine");
            basicFont = content.Load<SpriteFont>("fonts/BasicFont");

            dot = content.Load<Texture2D>("effects/basic_particle");
            effectHandler.LoadContent(content);

            currentPreparationState = StagePreparationStates.CHECK_KINECT_CONNECTION;

            myUI.LoadContent(content);
        }

        /* ===================================================================
         * UPDATE METHODS
         * ===================================================================
         */ 
        public override void Update()
        {
            base.UpdateComponents();

            switch(currentState){
                case StageScreenStates.PREPARING:
                    //see in Draw function the calling of the function prepareStageUI
                    if (currentPreparationState == StagePreparationStates.FINISHED)
                    {
                        currentState = StageScreenStates.RUNNING;
                    }
                    break;
                case StageScreenStates.RUNNING:
                    updateGame();
                    effectHandler.Update();
                    break;
                case StageScreenStates.PAUSED:
                    if (kinector.isTrackingSkeleton())
                    {
                        resumeCtr = GameConfig.RESUME_COUNT;
                        currentState = StageScreenStates.RESUMING;
                    }
                    break;
                case StageScreenStates.RESUMING:
                    resumeCtr--;
                    if (resumeCtr <= 0)
                    {
                        currentState = StageScreenStates.RUNNING;
                    }
                    break;
                case StageScreenStates.ENDING:
                    //do something
                    break;
            }

        }

        private void updateGame()
        {
            updateInterval();

            if (kinector.isTrackingSkeleton() || usingMouseAsInput)
            {
                switch( currentGame ){
                    case TAPGame.SEQUENTIAL_SOLO:
                        switch (gameMode)
                        {
                            case GameSequentialSolo.SOLO_BUBBLES:
                                updateSoloBubbles();
                                break;
                            case GameSequentialSolo.DRAG_BUBBLE:
                                updateDragBubbles();
                                break;
                            case GameSequentialSolo.SET_BUBBLES:
                                updateBubbleSets();
                                break;
                        }
                        break;
                    case TAPGame.SEQUENTIAL_MIXED:
                        updateSoloBubbles();
                        updateDragBubbles();
                        updateBubbleSets();
                        break;
                }
            }
            else
            {
                currentState = StageScreenStates.PAUSED;
            }
        }

        private void updateDragBubbles()
        {
            for (iter1 = 0; iter1 < dragBubblesOnScreen.Count; iter1++)
            {
                if (dragBubblesOnScreen[iter1].currentState != DragBubbleState.REMOVAL_STATE)
                {
                    //update the bubble's state
                    dragBubblesOnScreen[iter1].Update();

                    refBub = dragBubblesOnScreen[iter1].bubble1;

                    if (refBub.isActive())
                    {
                        switch (refBub.bubbleType)
                        {
                            case BubbleType.HAND:
                                if (dragBubblesOnScreen[iter1].currentState != DragBubbleState.LOCKED_IN)
                                {
                                    if (kinector.isColliding(refBub.collisionBox, JointType.HandRight))
                                    {
                                        refBub.setState(BubbleState.HIGHLIGHTED_STATE);
                                        refBub.jointHovering = JointType.HandRight;
                                        dragBubblesOnScreen[iter1].currentState = DragBubbleState.HOVERED;
                                    }
                                    else if (kinector.isColliding(refBub.collisionBox, JointType.HandLeft))
                                    {
                                        refBub.setState(BubbleState.HIGHLIGHTED_STATE);
                                        refBub.jointHovering = JointType.HandLeft;
                                        dragBubblesOnScreen[iter1].currentState = DragBubbleState.HOVERED;
                                    }
                                    else
                                    {
                                        refBub.setState(BubbleState.NORMAL_STATE);
                                        dragBubblesOnScreen[iter1].currentState = DragBubbleState.NORMAL;
                                        allHighlighted = false;
                                    }
                                }
                                else
                                {   
                                 
                                    dragBubblesOnScreen[iter1].follow(refBub.jointHovering==JointType.HandRight?kinector.getHandPosition(JointType.HandRight):kinector.getHandPosition(JointType.HandLeft));
                                    
                                }
                                break;
                        }
                    }

                    dragBubblesOnScreen[iter1].resetPopCounters();
                    
                }
                else
                {
                    dragBubblesOnScreen.RemoveAt(iter1--);
                    continue;
                }

            }
        }

        private void updateBubbleSets()
        {
            for (iter1 = 0; iter1 < bubbleSetsOnScreen.Count; iter1++)
            {
                if (bubbleSetsOnScreen[iter1].currentState != BubbleSetState.REMOVAL_STATE)
                {
                    //update the bubble's state
                    bubbleSetsOnScreen[iter1].Update();

                    //we assume first that all bubbles are touched until we found one that isn't
                    allHighlighted = true;

                    for (iter3 = 0; iter3 < bubbleSetsOnScreen[iter1].bubbles.Count; iter3++)
                    {
                        refBub = bubbleSetsOnScreen[iter1].bubbles[iter3];

                        if (refBub.isActive())
                        {
                            switch (refBub.bubbleType)
                            {
                                case BubbleType.HAND:
                                    if (kinector.isColliding(refBub.collisionBox, JointType.HandRight) || kinector.isColliding(refBub.collisionBox, JointType.HandLeft))
                                    {
                                        refBub.setState(BubbleState.HIGHLIGHTED_STATE);
                                    }
                                    else
                                    {
                                        refBub.setState(BubbleState.NORMAL_STATE);
                                        allHighlighted = false;
                                    }
                                    break;
                            }
                        }
                    }

                    if (!allHighlighted)
                    {
                        //we start counting to popping only when all bubbles are highlighted/touched
                        bubbleSetsOnScreen[iter1].resetPopCounters();
                    }
                }
                else
                {
                    bubbleSetsOnScreen.RemoveAt(iter1--);
                    continue;
                }
               
            }
        }

        private void updateSoloBubbles()
        {
            for (iter1 = 0; iter1 < bubblesOnScreen.Count; iter1++)
            {
                refBub = bubblesOnScreen[iter1];
                refBub.Update();

                if (refBub.isReadyForRemoval())
                {
                    bubblesOnScreen.RemoveAt(iter1--);
                    continue;

                }
                else if ( refBub.isActive() )
                {
                    switch (refBub.bubbleType)
                    {
                        case BubbleType.HAND:
                            if (kinector.isColliding(refBub.collisionBox, JointType.HandRight) || kinector.isColliding(refBub.collisionBox, JointType.HandLeft))
                            {
                                refBub.setState(BubbleState.HIGHLIGHTED_STATE);
                                if (refBub.popCounter >= refBub.popTime-1)
                                {
                                    effectHandler.addEffect((int)refBub.xPos, (int)refBub.yPos, 20);
                                    effectHandler.addEffect((int)refBub.xPos, (int)refBub.yPos, 10, Color.White);
                                }
                            }
                            else
                            {
                                refBub.setState(BubbleState.NORMAL_STATE);
                            }
                            break;
                    }
                }
            }
        }

        private void updateInterval()
        {
            switch( currentGame ){
                case TAPGame.SEQUENTIAL_SOLO:
                    updateGameSequentialSolo();
                    break;
                case TAPGame.SEQUENTIAL_MIXED:
                    if (bubbleSetsOnScreen.Count + bubblesOnScreen.Count + dragBubblesOnScreen.Count == 0)
                    {
                        int randIndex, randIndex2, combination;
                        Vector2 pos, pos2;
                        Vector2 rightShoulderPos = kinector.getRightShoulderPosition();
                        Vector2 leftShoulderPos = kinector.getLeftShoulderPosition();
                        for (int count = 0; count < 4; count++)
                        {
                            combination = Randomizer.random(1, 5);

                            switch (combination)
                            {
                                case 1:
                                    randIndex = Randomizer.random(0, leftReachablePoints.Count - 1);
                                    pos = leftReachablePoints[randIndex] + leftShoulderPos;
                                    createSoloBubble(pos);
                                    leftReachablePoints.RemoveAt(randIndex);
                                    break;
                                case 2:
                                    randIndex = Randomizer.random(0, rightReachablePoints.Count - 1);
                                    pos = rightReachablePoints[randIndex] + rightShoulderPos;
                                    createSoloBubble(pos);
                                    rightReachablePoints.RemoveAt(randIndex);
                                    break;
                                case 3:
                                    randIndex = Randomizer.random(0, leftReachablePoints.Count - 1);
                                    randIndex2 = Randomizer.random(0, rightReachablePoints.Count - 1);
                                    pos = leftReachablePoints[randIndex] + leftShoulderPos;
                                    pos2 = rightReachablePoints[randIndex2] + rightShoulderPos;
                                    createBubbleSet(pos, pos2);
                                    leftReachablePoints.RemoveAt(randIndex);
                                    rightReachablePoints.RemoveAt(randIndex2);
                                    count += 1; //we add 1 to count since we created 2 bubbles
                                    break;
                                case 4:
                                    randIndex = Randomizer.random(0, leftReachablePoints.Count - 1);
                                    randIndex2 = Randomizer.random(0, rightReachablePoints.Count - 1);
                                    pos = leftReachablePoints[randIndex] + leftShoulderPos;
                                    pos2 = rightReachablePoints[randIndex2] + rightShoulderPos;
                                    createDragBubble(pos, pos2);
                                    leftReachablePoints.RemoveAt(randIndex);
                                    rightReachablePoints.RemoveAt(randIndex2);
                                    count += 1; //we add 1 to count since we created 2 bubbles
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        private void updateGameSequentialSolo()
        {
            switch (gameMode)
            {
                case GameSequentialSolo.SOLO_BUBBLES:
                    bubbleCtr--;
                    if (soloBubblesToPop > 0)
                    {
                        if (bubbleCtr <= 0)
                        {
                            createRandomBubble();
                            bubbleCtr = bubbleInterval;
                        }
                        else if (bubbleCtr > GameConfig.MIN_GAP_BETWEEN_BUBBLES && bubblesOnScreen.Count == 0)
                        {
                            bubbleCtr = GameConfig.MIN_GAP_BETWEEN_BUBBLES;
                        }
                    }
                    else
                    {
                        if (bubblesOnScreen.Count <= 0)
                        {
                            gameMode = GameSequentialSolo.SET_BUBBLES;
                        }
                    }
                    break;
                case GameSequentialSolo.DRAG_BUBBLE:
                    dragBubbleCtr--;

                    if (dragBubblesToPop > 0)
                    {
                        if (dragBubbleCtr <= 0)
                        {
                            createDragBubble();
                            dragBubbleCtr = dragBubbleInterval;
                        }
                        else if (dragBubbleCtr > GameConfig.MIN_GAP_BETWEEN_BUBBLES && dragBubblesOnScreen.Count == 0)
                        {
                            dragBubbleCtr = GameConfig.MIN_GAP_BETWEEN_BUBBLES;
                        }
                    }
                    else
                    {
                        if (dragBubblesOnScreen.Count <= 0)
                        {
                            currentState = StageScreenStates.ENDING;
                        }
                    }
                    break;
                case GameSequentialSolo.SET_BUBBLES:
                    bubbleSetCtr--;
                    if (setBubblesToPop > 0)
                    {
                        if (bubbleSetCtr <= 0)
                        {
                            createBubbleSet();
                            bubbleSetCtr = bubbleSetInterval;
                        }
                        else if (bubbleSetCtr > GameConfig.MIN_GAP_BETWEEN_BUBBLES && bubbleSetsOnScreen.Count == 0)
                        {
                            bubbleSetCtr = GameConfig.MIN_GAP_BETWEEN_BUBBLES;
                        }
                    }
                    else
                    {
                        if (bubbleSetsOnScreen.Count <= 0)
                        {
                            gameMode = GameSequentialSolo.DRAG_BUBBLE;
                        }
                    }
                    break;
            }
        }

        /* ======================================================================
         * BUBBLE CREATORS
         * ======================================================================
         */

        //creates a drag bubble
        private void createDragBubble(float minDistance = 100)
        {
            dragBubblesOnScreen.Add( new DragBubble(DragBubbleType.HAND, blueHands, directionLineTexture, effectHandler) );
            dragBubblesToPop--;
        }

        private void createDragBubble(Vector2 pos1, Vector2 pos2)
        {
            dragBubblesOnScreen.Add(new DragBubble(DragBubbleType.HAND, blueHands, directionLineTexture, effectHandler, pos1, pos2));
            dragBubblesToPop--;
        }
        //creates a set bubble
        private void createBubbleSet(float minDistance = 100)
        {
            bubbleSetsOnScreen.Add(new BubbleSet(BubbleSetType.HANDHAND, blueHands, lineTexture, effectHandler));
            setBubblesToPop--;
        }

        private void createBubbleSet(Vector2 pos1, Vector2 pos2)
        {
            bubbleSetsOnScreen.Add(new BubbleSet(BubbleSetType.HANDHAND,blueHands,lineTexture,effectHandler,pos1,pos2));
            setBubblesToPop--;
        }
        //creates a random bubble
        private void createRandomBubble()
        {
            int randInt = Randomizer.random(0,rightReachablePoints.Count-1);
            Vector2 randVector2 = rightReachablePoints.ElementAt(randInt) + kinector.getRightShoulderPosition();
            bubblesOnScreen.Add(new Bubble(randVector2.X, randVector2.Y, blueHands, BubbleType.HAND));

            rightReachablePoints.RemoveAt(randInt);
            soloBubblesToPop--;
        }

        private void createSoloBubble(Vector2 position)
        {
            bubblesOnScreen.Add(new Bubble(position.X, position.Y, blueHands, BubbleType.HAND));
            soloBubblesToPop--;
        }

        /*
         * ==========================================================================
         * USER INTERFACE HANDLERS
         * ==========================================================================
         */
 
        //draw function.
        public override void Draw(SpriteBatch sprite)
        {
            switch (currentState)
            {
                case StageScreenStates.PREPARING:
                    //myUI.Draw(sprite, currentState);
                    PrepareStageUI(sprite);
                    break;
                case StageScreenStates.RUNNING:
                    DrawBubbles(sprite);
                    effectHandler.Draw(sprite);
                    for (int i = 0; i < rightReachablePoints.Count; i++)
                    {
                        sprite.Draw(dot, rightReachablePoints[i] + kinector.getRightShoulderPosition(), Color.White * 0.25f);
                    }

                    for (int i = 0; i < leftReachablePoints.Count; i++)
                    {
                        sprite.Draw(dot, leftReachablePoints[i] + kinector.getLeftShoulderPosition(), Color.White * 0.25f);
                    }
                    break;
                case StageScreenStates.PAUSED:
                    myUI.writeNotificationWindow(sprite, "PAUSED", "Unable to track user. No skeleton tracked.", currentState);
                    myUI.Draw(sprite, currentState);
                    break;
                case StageScreenStates.RESUMING:
                    myUI.writeNotificationWindow(sprite, "RESUMING", "in " + resumeCtr / 80 + " seconds.", currentState);
                    DrawBubbles(sprite);
                    myUI.Draw(sprite, currentState);
                    break;
            }

            base.DrawTransitionBox(sprite);

            
        }

        //draws all bubbles that are on screen
        private void DrawBubbles(SpriteBatch sprite)
        {
            for (iter2 = 0; iter2 < bubblesOnScreen.Count; iter2++)
            {
                bubblesOnScreen[iter2].Draw(sprite);
            }

            for (iter2 = 0; iter2 < bubbleSetsOnScreen.Count; iter2++)
            {
                bubbleSetsOnScreen[iter2].Draw(sprite);
            }

            for (iter2 = 0; iter2 < dragBubblesOnScreen.Count; iter2++)
            {
                dragBubblesOnScreen[iter2].Draw(sprite);
            }

            
        }

        //preparationWindow
        private void PrepareStageUI(SpriteBatch sprite)
        {
            switch (currentPreparationState)
            {
                case StagePreparationStates.CHECK_KINECT_CONNECTION:
                    if (kinector.getConnectionStatus() != KinectStatus.Connected)
                    {
                        myUI.writeNotificationWindow(sprite, "NO KINECT SENSOR DETECTED", "Kinect device not properly connected.", Color.Red, Color.White, Color.LightGray);
                        //myUI.WriteToPreparationWindow(sprite, "Kinect device not properly connected.");
                    }
                    else
                    {
                        myUI.writeNotificationWindow(sprite, "PREPARING SENSOR", "Kinect device ready.", Color.Yellow, Color.White, Color.LightGray);
                        //myUI.WriteToPreparationWindow(sprite, "Kinect device ready");
                    }

                    msgCtr += 1;
                    if (msgCtr >= GameConfig.MESSAGE_SWITCH_TIME)
                    {
                        msgCtr = 0;
                        if (kinector.getConnectionStatus() == KinectStatus.Connected)
                        {
                            currentPreparationState = StagePreparationStates.CHECK_SKELETON;
                        }
                        else
                        {
                            msgCtr = 200;
                            currentPreparationState = StagePreparationStates.READY;
                        }
                        
                    }
                    break;
                case StagePreparationStates.CHECK_SKELETON:
                    if (!kinector.hasFoundSkeleton())
                    {
                        myUI.writeNotificationWindow(sprite, "UNABLE TO DETECT USER", "No user tracked yet.", Color.OrangeRed, Color.White, Color.LightGray);
                    }
                    else
                    {
                        myUI.writeNotificationWindow(sprite, "CALIBRATING", "A user is now being tracked", Color.Yellow, Color.White, Color.LightGray);
                        
                        msgCtr += 1;
                        if (msgCtr >= GameConfig.MESSAGE_SWITCH_TIME)
                        {
                            msgCtr = 0;
                            currentPreparationState = StagePreparationStates.CHECK_DEPTH;
                        }
                    }
                    break;
                case StagePreparationStates.CHECK_DEPTH:
                    if (kinector.hasFoundSkeleton())
                    {
                        if (kinector.getDepth(JointType.HipCenter) < 2f)
                        {
                            myUI.writeNotificationWindow(sprite, "ADJUST USER POSITION", "Please move farther away from the sensor.", Color.OrangeRed, Color.White, Color.LightGray);
                        }
                       
                        else
                        {
                            myUI.writeNotificationWindow(sprite, "IDEAL POSITION ACHIEVED", "1.5 meters away from sensor", Color.OrangeRed, Color.White, Color.LightGray);
                            msgCtr = 0;
                            currentPreparationState = StagePreparationStates.ADJUST_ANGLE;
                        }

                    }
                    else
                    {
                        currentPreparationState = StagePreparationStates.CHECK_SKELETON;
                        msgCtr = 0;
                    }

                    break;
                case StagePreparationStates.ADJUST_ANGLE:
                    if (kinector.hasFoundSkeleton())
                    {
                        if (kinector.getHeadPosition().Y > GameConfig.APP_HEIGHT/4 )
                        {
                            if (msgCtr <= 0)
                            {
                                kinector.setElevationAngle(kinector.getElevationAngle() - 3);
                                msgCtr = GameConfig.ANGLE_ADJUST_WAIT_TIME;
                                
                            }
                            else
                            {
                                myUI.writeNotificationWindow(sprite, "ADJUSTING ANGLE", "Please wait...", Color.OrangeRed, Color.White, Color.LightGray);
                                msgCtr--;
                            }

                        }
                        else if (kinector.getCentralHipPosition().Y < GameConfig.APP_HEIGHT / 2)
                        {
                            if (msgCtr <= 0)
                            {
                                kinector.setElevationAngle(kinector.getElevationAngle() + 3);
                                msgCtr = GameConfig.ANGLE_ADJUST_WAIT_TIME;

                            }
                            else
                            {
                                myUI.writeNotificationWindow(sprite, "ADJUSTING CAMERA ANGLE", "Please wait...", Color.OrangeRed, Color.White, Color.LightGray);
                                msgCtr--;
                            }

                        }
                        else
                        {
                            msgCtr = 0;
                            currentPreparationState = StagePreparationStates.MEASURE_SEGMENTS;
                        }

                    }
                    else
                    {
                        currentPreparationState = StagePreparationStates.CHECK_SKELETON;
                        msgCtr = 0;
                    }

                    break;
                case StagePreparationStates.MEASURE_SEGMENTS:
                    myUI.writeNotificationWindow(sprite, "SCALING BODY SEGMENTS", "Please Remain still.", Color.OrangeRed, Color.White, Color.LightGray);
                    msgCtr += 1;
                    
                    //measure right limb size by getting mean average
                    rightLimbEstSize += MathUtil.getDistance(kinector.getRightShoulderPosition(), kinector.getRightElbowPosition()) + MathUtil.getDistance(kinector.getRightElbowPosition(),kinector.getHandPosition());
                    rightLimbNCount += 1;

                    //measure left limb size by getting mean average
                    leftLimbEstSize += MathUtil.getDistance(kinector.getRightShoulderPosition(), kinector.getRightElbowPosition()) + MathUtil.getDistance(kinector.getRightElbowPosition(),kinector.getHandPosition());
                    leftLimbNCount += 1;

                    if (msgCtr >= GameConfig.MESSAGE_SWITCH_TIME + 150)
                    {
                        //right limb mean
                        rightLimbEstSize = rightLimbEstSize / rightLimbNCount + 30;
                        rightReachablePoints = Randomizer.getEvenListOfPoints(rightLimbEstSize, 40, 40);

                        //left limb mean
                        leftLimbEstSize = leftLimbEstSize / leftLimbNCount + 30;
                        leftReachablePoints = Randomizer.getEvenListOfPoints(leftLimbEstSize, 40, 40);

                        soloBubblesToPop = rightReachablePoints.Count;
                        msgCtr = 300;
                        currentPreparationState = StagePreparationStates.READY;
                    }
                    break;
                case StagePreparationStates.READY:
                    myUI.writeNotificationWindow(sprite, "READY", "Starting in " + msgCtr / 80 + " second/s", Color.LightYellow, Color.White, Color.LightGray);
                    msgCtr -= 1;
                    if (msgCtr <= 80)
                    {
                        if (rightLimbEstSize == 0 || leftLimbEstSize == 0)
                        {
                            //right limb mean
                            rightLimbEstSize = 200;
                            rightReachablePoints = Randomizer.getEvenListOfPoints(rightLimbEstSize, 40, 40);

                            //left limb mean
                            leftLimbEstSize = 200;
                            leftReachablePoints = Randomizer.getEvenListOfPoints(leftLimbEstSize, 40, 40);
                        }
                        currentPreparationState = StagePreparationStates.FINISHED;
                    }
                    break;
                case StagePreparationStates.FINISHED:
                    break;

            }
        }
    }
}
