#include "appEntry.h"
#include "Scene/scene.h"
#include <iostream>

int GDCPhysics::appEntry() {
    // init SDL
    if(SDL_Init(SDL_INIT_EVERYTHING) != 0) {
        printf("ERROR SDL_Init\n");
        return -1;
    }

    // create a window
    SDL_Window * window = SDL_CreateWindow(
                                           "PingPong",             // window title
                                           SDL_WINDOWPOS_CENTERED,     // x position, centered
                                           SDL_WINDOWPOS_CENTERED,     // y position, centered
                                           640,                        // width, in pixels
                                           480,                        // height, in pixels
                                           SDL_WINDOW_OPENGL | SDL_WINDOW_SHOWN  /*| SDL_WINDOW_RESIZABLE  |  SDL_WINDOW_ALLOW_HIGHDPI */   // flags
                                           );
    
    int window_cx=1;
    int window_cy=1;
    SDL_GetWindowSize(window, &window_cx, &window_cy);
    SDL_GL_GetDrawableSize(window, &window_cx, &window_cy);
    
    if( window == NULL ) {
        std::cout << "ERROR SDL_CreateWindow" << std::endl;
        return -1;
    }

    //shared context
    //http://forums.libsdl.org/viewtopic.php?t=9036&sid=3374c819e18df779e17b4ce5a49fdd15
    SDL_GL_SetAttribute(SDL_GL_SHARE_WITH_CURRENT_CONTEXT, 1);

    //Create context
    SDL_GLContext context = SDL_GL_CreateContext( window );
    if( context == NULL ) {
        std::cout << "ERROR SDL_GL_CreateContext" << std::endl;
        SDL_DestroyWindow(window);
        SDL_Quit();
        return -1;
    }

    SDL_GL_MakeCurrent(window, context);
    
    Scene& gameScene = Scene::GetInstance();
    gameScene.InitScene(window, window_cx, window_cy);

    bool quit=false;
    //While application is running
    while( !quit ) {
        SDL_Event e;
        //Handle events on queue
        while( SDL_PollEvent( &e ) != 0 ) {
            //User requests quit
            if( e.type == SDL_QUIT ) {
                quit = true;
            } else {
                Uint32 event_wndID=e.window.windowID;
                processSDLEvent(SDL_GetWindowFromID(event_wndID), e, &gameScene);
            }
        }

        //rendering loop
        gameScene.Update();
        gameScene.Render();
        
        SDL_Delay(5);
        
        //swapbuffer
        SDL_GL_SwapWindow( window );
    }
    
    //clean up
    SDL_GL_DeleteContext(context);  //DeleteContext will be called by renderer. So no need to call it here.
    SDL_DestroyWindow(window);
    SDL_Quit();
        
    return 0;
}

void GDCPhysics::processSDLEvent(SDL_Window * window, SDL_Event& e, void* userData) {
    Scene* scene = static_cast<Scene*>(userData);
    
    if(e.type==SDL_WINDOWEVENT) {
        SDL_WindowEvent* windowEvent = (SDL_WindowEvent*)&e;

        switch (windowEvent->event) {
            case SDL_WINDOWEVENT_RESIZED:
            case SDL_WINDOWEVENT_MAXIMIZED:
            case SDL_WINDOWEVENT_MINIMIZED:
            case SDL_WINDOWEVENT_RESTORED:
            case SDL_WINDOWEVENT_SIZE_CHANGED: {
                int window_cx=1;
                int window_cy=1;
                SDL_GetWindowSize(window, &window_cx, &window_cy);
                SDL_GL_GetDrawableSize(window, &window_cx, &window_cy);
                scene->Resize(window_cx, window_cy);
            }
                break;
            
            case SDL_WINDOWEVENT_FOCUS_LOST: {
                printf("SDL_WINDOWEVENT_FOCUS_LOST\n");
            }
                break;
            case SDL_WINDOWEVENT_FOCUS_GAINED: {
                printf("SDL_WINDOWEVENT_FOCUS_GAINED\n");
            }
                break;
                
            case SDL_WINDOWEVENT_SHOWN: {
                printf("SDL_WINDOWEVENT_SHOWN\n");
            }
                break;

            case SDL_WINDOWEVENT_EXPOSED: {
                printf("SDL_WINDOWEVENT_EXPOSED\n");
            }
                break;

            default:
                break;
        }
    } else if(e.type==SDL_KEYDOWN) {
        SDL_KeyboardEvent* keyBoardEvent = (SDL_KeyboardEvent*)&e;
        if (keyBoardEvent->keysym.scancode == SDL_SCANCODE_UP) {
            scene->MoveStrickerUP(true);
        }
        if (keyBoardEvent->keysym.scancode == SDL_SCANCODE_DOWN) {
            scene->MoveStrickerDown(true);
        }
    } else if(e.type==SDL_KEYUP) {
        SDL_KeyboardEvent* keyBoardEvent = (SDL_KeyboardEvent*)&e;
        if (keyBoardEvent->keysym.scancode == SDL_SCANCODE_UP) {
            scene->MoveStrickerUP(false);
        }
        if (keyBoardEvent->keysym.scancode == SDL_SCANCODE_DOWN) {
            scene->MoveStrickerDown(false);
        }
    } else if(e.type==SDL_MOUSEBUTTONDOWN) {
        int mouse_x = 0, mouse_y = 0;
        SDL_GetMouseState( &mouse_x, &mouse_y );

        SDL_MouseButtonEvent* mouseBtnEvent = (SDL_MouseButtonEvent*)&e;
        switch (mouseBtnEvent->button) {
            case SDL_BUTTON_LEFT: {
            }
                break;
            case SDL_BUTTON_MIDDLE: {
            }
                break;
            case SDL_BUTTON_RIGHT: {
            }
                break;
            default:
                break;
        }
    } else if(e.type==SDL_MOUSEBUTTONUP) {
        int mouse_x = 0, mouse_y = 0;
        SDL_GetMouseState( &mouse_x, &mouse_y );

        SDL_MouseButtonEvent* mouseBtnEvent = (SDL_MouseButtonEvent*)&e;
        switch (mouseBtnEvent->button) {
            case SDL_BUTTON_LEFT: {
                scene->MouseBtnUp();
            }
                break;
            case SDL_BUTTON_MIDDLE: {
            }
                break;
            case SDL_BUTTON_RIGHT: {
            }
                break;
            default:
                break;
        }
    } else if(e.type==SDL_MOUSEMOTION) {
        int mouse_x = 0, mouse_y = 0;
        SDL_GetMouseState( &mouse_x, &mouse_y );
        SDL_MouseMotionEvent* mouseMotionEvent = (SDL_MouseMotionEvent*)&e;
        switch (mouseMotionEvent->state) {
            case 1:
                break;
            case 2:
                break;
            case 4:
                break;

            default:
                break;
        }
    } else if(e.type==SDL_MOUSEWHEEL) {
    } else if (e.type==SDL_DROPFILE) {
    }
}
