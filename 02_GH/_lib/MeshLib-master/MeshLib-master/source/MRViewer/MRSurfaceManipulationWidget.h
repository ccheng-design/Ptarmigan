#pragma once
#include "MRViewerFwd.h"
#include "MRViewerEventsListener.h"
#include "MRMesh/MRMeshFwd.h"
#include "MRMesh/MRChangeMeshAction.h"
#include "MRMesh/MREnums.h"
#include "MRViewer/MRViewport.h"
#include <chrono>

namespace MR
{

/// @brief widget for surface modifying
/// @detail available 3 modes:
/// add (move surface region in direction of normal)
/// remove (move surface region in opposite direction to normal)
/// relax (relax surface region)
class MRVIEWER_CLASS SurfaceManipulationWidget :
    public MultiListener<MouseDownListener, MouseMoveListener, MouseUpListener,
                         PostDrawListener>
{
public:
    MRVIEWER_API SurfaceManipulationWidget();
    MRVIEWER_API ~SurfaceManipulationWidget();

    /// widget work modes
    enum class WorkMode
    {
        Add,
        Remove,
        Relax,
        Laplacian,
        Patch
    };

    /// Mesh change settings
    struct Settings
    {
        WorkMode workMode = WorkMode::Add;
        float radius = 1.f; // radius of editing region
        float relaxForce = 0.2f; // speed of relaxing, typical values (0 - 0.5]
        float editForce = 1.f; // the force of changing mesh
        float sharpness = 50.f; // effect of force on points far from center editing area. [0 - 100]
        float relaxForceAfterEdit = 0.25f; //  force of relaxing modified area after editing (add / remove) is complete. [0 - 0.5], 0 - not relax
        EdgeWeights edgeWeights = EdgeWeights::Cotan; // edge weights for Laplacian and Patch
    };

    /// initialize widget according ObjectMesh
    MRVIEWER_API void init( const std::shared_ptr<ObjectMesh>& objectMesh );
    /// reset widget state
    MRVIEWER_API void reset();

    /// set widget settings (mesh change settings)
    MRVIEWER_API void setSettings( const Settings& settings );
    /// get widget settings 
    MRVIEWER_API const Settings& getSettings() { return settings_; };

    // mimum radius of editing area.
    MRVIEWER_API float getMinRadius() { return minRadius_; };

    // get palette used for visualization point shifts
    Palette& palette() { return *palette_; }
    // update texture used for colorize surface (use after change colorMap in palette)
    MRVIEWER_API void updateTexture();
    // update texture uv coords used for colorize surface (use after change ranges in palette)
    MRVIEWER_API void updateUVs();
    // enable visualization of mesh deviations
    MRVIEWER_API void enableDeviationVisualization( bool enable );
    // get min / max point shifts for (usefull for setup palette)
    MRVIEWER_API Vector2f getMinMax();
private:
    /// start modifying mesh surface
    MRVIEWER_API bool onMouseDown_( MouseButton button, int modifiers ) override;
    /// stop modifying mesh surface, generate history action
    MRVIEWER_API bool onMouseUp_( MouseButton button, int modifiers ) override;
    /// update
    MRVIEWER_API bool onMouseMove_( int mouse_x, int mouse_y ) override;
    /// need to visualize bad region (draw grey circle)
    MRVIEWER_API void postDraw_() override;

    void reallocData_( size_t size );
    void clearData_();

    void initConnections_();
    void resetConnections_();

    void changeSurface_();
    void updateUVmap_( bool set );
    void updateRegion_( const Vector2f& mousePos );
    void abortEdit_();
    // Laplacian
    void laplacianPickVert_( const PointOnFace& pick );
    void laplacianMoveVert_( const Vector2f& mousePos );

    void updateVizualizeSelection_( const ObjAndPick& objAndPick );

    void updateRegionUVs_( const VertBitSet& region );
    void updateValueChanges_( const VertBitSet& region );
    void updateValueChangesByDistance_( const VertBitSet& region );

    Settings settings_;

    std::shared_ptr<ObjectMesh> obj_;
    float minRadius_ = 1.f;
    Vector2f mousePos_; ///< mouse position of last updateRegion_
    VertBitSet singleEditingRegion_;  ///< current (under the cursor) region of tool application
    VertBitSet visualizationRegion_;  ///< vertices of triangles partially or fully highlighted with red
    VertBitSet generalEditingRegion_; ///< united region of tool application since the last mouse down
    VertScalars pointsShift_;
    VertScalars editingDistanceMap_;
    VertScalars visualizationDistanceMap_;
    VertBitSet changedRegion_;
    VertScalars valueChanges_;
    VertScalars lastStableValueChanges_;
    std::shared_ptr<Mesh> originalMesh_; ///< original input mesh
    VertBitSet unknownSign_; ///< cached data to avoid reallocating memory
    std::shared_ptr<ObjectMesh> lastStableObjMesh_;
    bool firstInit_ = true; // need to save settings in re-initial
    bool badRegion_ = false; // in selected region less than 3 points

    bool mousePressed_ = false;

    std::chrono::time_point<std::chrono::high_resolution_clock> timePoint_;
    boost::signals2::scoped_connection meshChangedConnection_;
    bool ownMeshChangedSignal_ = false;

    bool connectionsInitialized_ = false;

    // Laplacian
    VertId touchVertId_; // we fix this vertex in Laplacian and move it manually
    Vector3f touchVertIniPos_; // initial position of fixed vertex
    Vector2i storedDown_;
    std::unique_ptr<Laplacian> laplacian_;
    std::shared_ptr<HistoryAction> historyAction_; // this action is prepared beforehand for better responsiveness, but pushed only on mouse move
    bool appendHistoryAction_ = false;

    std::shared_ptr<Palette> palette_;
    bool enableDeviationTexture_ = true;
};

}
